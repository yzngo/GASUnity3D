using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbilitySystem.Abilities;
using AbilitySystem.GameplayEffects;
using UnityEngine;
using AbilitySystem.Attributes;
using AbilitySystem.Cues;
using UnityEngine.Events;
using AbilitySystem.Interfaces;

namespace AbilitySystem {

    /// The ASC is the primary component of the GAS. Every game object 
    /// that needs to participate with the GAS needs to have this component attached.
    [AddComponentMenu("Gameplay Ability System/Ability System")]
    public class AbilitySystemComponent : MonoBehaviour {

        // 自己身上作为目标的点
        [SerializeField] private Transform targetPoint = default;
        public Transform TargetPoint => targetPoint;

        // Called when a GameplayEvent is executed
        public GameplayEvent OnGameplayEvent => onGameplayEvent;

        // Called when an Ability is activated(激活) on this ASC
        // public GenericAbilityEvent OnGameplayAbilityActivated => onGameplayAbilityActivated;
        // Called when an Ability is committed(提交) on this ASC
        // public GenericAbilityEvent OnGameplayAbilityCommitted => onGameplayAbilityCommitted;
        // Called when an Ability ends
        // public GenericAbilityEvent OnGameplayAbilityEnded => onGameplayAbilityEnded;
        /// Called when an effect is added
        // public GenericGameplayEffectEvent OnEffectAdded => onEffectAdded;
        /// Called when an effect is removed
        // public GenericGameplayEffectEvent OnEffectRemoved => onEffectRemoved;

        /// List of running abilities that have not ended 
        // public List<IGameplayAbility> RunningAbilities => runningAbilities;

        // Lists all active GameplayEffect
        public ActiveGameplayEffectsContainer ActiveEffectsContainer => activeEffectsContainer;

        private GameplayEvent onGameplayEvent = new GameplayEvent();
        // private GenericAbilityEvent onGameplayAbilityActivated = new GenericAbilityEvent(); 
        // private GenericAbilityEvent onGameplayAbilityCommitted = new GenericAbilityEvent();
        // private GenericAbilityEvent onGameplayAbilityEnded = new GenericAbilityEvent();
        private GenericGameplayEffectEvent onEffectAdded = new GenericGameplayEffectEvent();
        private GenericGameplayEffectEvent onEffectRemoved = new GenericGameplayEffectEvent();
        private List<IGameplayAbility> runningAbilities = new List<IGameplayAbility>();
        private ActiveGameplayEffectsContainer activeEffectsContainer;

        private Animator animator;
        public Animator Animator => animator;

        private AttributeSet attributeSet;
        public AttributeSet AttributeSet => attributeSet;

        public IEnumerable<GameplayTag> ActiveTags =>
                ActiveEffectsContainer
                            .ActiveEffectAttributeAggregator
                            .GetAllActiveEffects()
                            .SelectMany(x => x.Effect.EffectTags.GrantedToASCTags.Added)
                            .Union(AbilityGrantedTags);

        private IEnumerable<GameplayTag> AbilityGrantedTags => runningAbilities.SelectMany(x => x.Tags.ActivationOwnedTags.Added);

        public void Awake() {
            activeEffectsContainer = new ActiveGameplayEffectsContainer(this);
            animator = GetComponent<Animator>();
            attributeSet = GetComponent<AttributeSet>();
        }

        // public void HandleGameplayEvent(GameplayTag EventTag, GameplayEventData Payload) {
            /**
             * TODO: Handle triggered abilities
             * Search component for all abilities that are automatically triggered from a gameplay event
             */

            // OnGameplayEvent.Invoke(EventTag, Payload);
        // }

        public void NotifyAbilityEnded(GameplayAbility ability) {
            runningAbilities.Remove(ability);
        }

        public bool TryActivateAbility(GameplayAbility Ability) {
            if (!CanActivateAbility(Ability)) return false;
            if (!Ability.IsAbilityActivatable(this)) return false;
            runningAbilities.Add(Ability);
            Ability.CommitAbility(this);

            return true;
        }

        public bool CanActivateAbility(IGameplayAbility Ability) {
            // Check if this ability is already active on this ASC
            if (runningAbilities.Contains(Ability)) {
                return false;
            }
            return true;
        }

        // public async void ApplyBatchGameplayEffects(IEnumerable<(GameplayEffect Effect, AbilitySystemComponent Target, float Level)> batchedGameplayEffects) {

        //     var instantEffects = batchedGameplayEffects.Where(x => x.Effect.GameplayEffectPolicy.DurationPolicy == DurationPolicy.Instant);
        //     var durationalEffects = batchedGameplayEffects.Where(
        //         x =>
        //             x.Effect.GameplayEffectPolicy.DurationPolicy == DurationPolicy.Duration ||
        //             x.Effect.GameplayEffectPolicy.DurationPolicy == DurationPolicy.Infinite
        //             );
        //     // Apply instant effects
        //     foreach (var item in instantEffects) {
        //         await ApplyEffectToTarget(item.Effect, item.Target);
        //     }
        //     // Apply durational effects
        //     foreach (var effect in durationalEffects) {
        //         if (await ApplyEffectToTarget(effect.Effect, effect.Target)) {

        //         }
        //     }
        // }

        public Task<GameplayEffect> ApplyEffectToTarget(GameplayEffect effect, AbilitySystemComponent target, float level = 0) {
            // TODO: Check to make sure all the attributes being modified by this gameplay effect exist on the target

            // TODO: Get list of tags owned by target

            // TODO: Check for immunity tags, and don't apply gameplay effect if target is immune (and also add Immunity Tags container to IGameplayEffect)

            // TODO: Check to make sure Application Tag Requirements are met (i.e. target has all the required tags, and does not contain any prohibited tags )
            if (!effect.ApplicationTagRequirementMet(target)) {
                return null;
            }

            // If this is a non-instant gameplay effect (i.e. it will modify the current value, not the base value)

            // If this is an instant gameplay effect (i.e. it will modify the base value)

            // Handling Instant effects is different to handling HasDuration and Infinite effects
            if (effect.GameplayEffectPolicy.DurationPolicy == DurationPolicy.Instant) {
                effect.ApplyInstantEffect(target);
            } else {
                // Durational effects require attention to many more things than instant effects
                // Such as stacking and effect durations
                var EffectData = new ActiveGameplayEffectData(effect, this, target);
                _ = target.ActiveEffectsContainer.ApplyGameEffect(EffectData);
            }

            // Remove all effects which have tags defined as "Remove Gameplay Effects With Tag". 
            // We do this by setting the expiry time on the effect to make it end prematurely
            // This is accomplished by finding all effects which grant these tags, and then adjusting start time
            var tagsToRemove = effect.EffectTags.BeRemovedEffectsTags.Added;
            var activeGEs = target.GetActiveEffectsTags()
                                    .Where(x => tagsToRemove.Any(y => x.Tag == y.Tag))
                                    .Join(tagsToRemove, x => x.Tag, x => x.Tag, (x, y) => new { Tag = x.Tag, EffectData = x.GrantingEffect, StacksToRemove = y.StacksToRemove })
                                    .OrderBy(x => x.EffectData.CooldownTimeRemaining);

            Dictionary<GameplayEffect, int> StacksRemoved = new Dictionary<GameplayEffect, int>();
            foreach (var GE in activeGEs) {
                if (!StacksRemoved.ContainsKey(GE.EffectData.Effect)) {
                    StacksRemoved.Add(GE.EffectData.Effect, 0);
                }
                var stacksRemoved = StacksRemoved[GE.EffectData.Effect];
                if (GE.StacksToRemove == 0 || stacksRemoved < GE.StacksToRemove) {
                    GE.EffectData.ForceEndEffect();
                }

                StacksRemoved[GE.EffectData.Effect]++;
            }


            var gameplayCues = effect.GameplayCues;
            // Execute gameplay cue
            for (var i = 0; i < gameplayCues.Count; i++) {
                var cue = gameplayCues[i];
                cue.HandleCue(target.gameObject, CueEventMoment.OnActive);
            }
            return Task.FromResult(effect);
        }

        public IEnumerable<(GameplayTag Tag, ActiveGameplayEffectData GrantingEffect)> GetActiveEffectsTags()
        {
            var activeEffects = ActiveEffectsContainer.ActiveEffectAttributeAggregator.GetAllActiveEffects();
            if (activeEffects == null) 
                return new List<(GameplayTag, ActiveGameplayEffectData)>();
            return activeEffects.SelectMany(x => x.Effect.GrantedTags.Select(y => (y, x)));
        }

// attribute -----------------------------------------------------------------------------
        public float GetBaseValue(AttributeType type) => GetAttributeByType(type).BaseValue;
        public float GetCurrentValue(AttributeType type) => GetAttributeByType(type).CurrentValue;
        public void SetBaseValue(AttributeType type, float value) => GetAttributeByType(type).SetBaseValue(attributeSet, ref value);
        public void SetCurrentValue(AttributeType type, float value) => GetAttributeByType(type).SetCurrentValue(attributeSet, ref value);
        private Attribute GetAttributeByType(AttributeType type) => attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == type);
    }

//----------------------------------------------------------------------------------------
    [System.Serializable]
    public class GenericGameplayEffectEvent : UnityEvent<GameplayEffect> {

    }
}
