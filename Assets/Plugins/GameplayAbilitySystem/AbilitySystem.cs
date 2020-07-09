using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameplayAbilitySystem.Abilities;
using GameplayAbilitySystem.Effects;
using UnityEngine;
using GameplayAbilitySystem.Attributes;
using GameplayAbilitySystem.Cues;
using UnityEngine.Events;
using GameplayAbilitySystem.Interfaces;

namespace GameplayAbilitySystem {

    /// The AbilitySytem is the primary component of the GAS. Every game object 
    /// that needs to participate with the GAS needs to have this component attached.
    [AddComponentMenu("Ability System/Ability System")]
    [RequireComponent(typeof(AttributeSet))]
    public class AbilitySystem : MonoBehaviour {

        // 自己身上作为目标的点
        [SerializeField] private Transform targetPoint = default;
        public Transform TargetPoint => targetPoint;

        // Called when a GameplayEvent is executed
        private AbilityEvent onGameplayEvent = new AbilityEvent();
        public AbilityEvent OnGameplayEvent => onGameplayEvent;

        private List<IGameplayAbility> runningAbilities = new List<IGameplayAbility>();

        // Lists all active Effect
        private ActiveEffectsContainer activeEffectsContainer;
        public ActiveEffectsContainer ActiveEffectsContainer => activeEffectsContainer;

        private Animator animator;
        public Animator Animator => animator;

        private AttributeSet attributeSet;

        public IEnumerable<GameplayTag> ActiveTags =>
                ActiveEffectsContainer
                            .ActiveEffectAttributeAggregator
                            .GetAllActiveEffects()
                            .SelectMany(x => x.Effect.EffectTags.GrantedToASCTags.Added)
                            .Union(AbilityGrantedTags);

        private IEnumerable<GameplayTag> AbilityGrantedTags => runningAbilities.SelectMany(x => x.Tags.ActivationOwnedTags.Added);

        public void Awake() {
            activeEffectsContainer = new ActiveEffectsContainer(this);
            animator = GetComponent<Animator>();
            attributeSet = GetComponent<AttributeSet>();
        }

        // Checks to see if the ability can be activated
        // DO NOT execute the ability
        // 激活之后后续流程交给ability
        public bool CanActivateAbility(GameplayAbility ability) {
            // Check if this ability is already active on this Ability System
            if (runningAbilities.Contains(ability)) {
                return false;
            }

            if (!ability.IsAbilityActivatable(this)) {
                return false;
            }
            return true;
        }

        // Try to activate the ability
        public bool TryActivateAbility(GameplayAbility ability, AbilitySystem target = null) {
            if (!CanActivateAbility(ability)) {
                return false;
            }
            // 一个技能的生命周期是从[释放开始]到[释放结束], 之后的工作交给effect
            // 技能仅仅是一个时序流程
            runningAbilities.Add(ability);
            ability.CommitAbility(this);

            GameplayTag abilityTag = ability.Tags.AbilityTags.Added.Count > 0 ? ability.Tags.AbilityTags.Added[0] : new GameplayTag();
            var data = new AbilityEventData();
            data.Target = target;
            onGameplayEvent?.Invoke(abilityTag, data);
            return true;
        }

        // Notifies this AbilitySystem that the specified ability has ended
        public void NotifyAbilityEnded(GameplayAbility ability) {
            runningAbilities.Remove(ability);
        }
        
        // Apply batched effect.
        // This can be useful if e.g. an ability applies a number of effects,
        // some with instant modifiers, and some with infinite or duration modifiers.
        // By batching these effects, we can ensure that all these effect happen 
        // with reference to the same base attribute value.
        public async void ApplyBatchGameplayEffects(IEnumerable<(GameplayEffect Effect, AbilitySystem Target, float Level)> batchedGameplayEffects) {

            var instantEffects = batchedGameplayEffects.Where(x => x.Effect.Policy.DurationPolicy == DurationPolicy.Instant);
            var durationalEffects = batchedGameplayEffects.Where(
                x =>
                    x.Effect.Policy.DurationPolicy == DurationPolicy.Duration ||
                    x.Effect.Policy.DurationPolicy == DurationPolicy.Infinite
                    );
            // Apply instant effects
            foreach (var item in instantEffects) {
                await ApplyEffectToTarget(item.Effect, item.Target);
            }
            // Apply durational effects
            foreach (var effect in durationalEffects) {
                if (await ApplyEffectToTarget(effect.Effect, effect.Target)) {
                }
            }
        }
        
        // effect也是ablity system管理
        // Apply a effect to the target
        // The overall effect may be modulated by the Level.
        // level -> maybe used to affect the "strength" of the effect
        public Task<GameplayEffect> ApplyEffectToTarget(GameplayEffect effect, AbilitySystem target, float level = 0) {
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
            if (effect.Policy.DurationPolicy == DurationPolicy.Instant) {
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
                cue.HandleCue(target, CueEventMomentType.OnActive);
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

        // private GenericGameplayEffectEvent onEffectAdded = new GenericGameplayEffectEvent();
        // private GenericGameplayEffectEvent onEffectRemoved = new GenericGameplayEffectEvent();

        // private GenericAbilityEvent onGameplayAbilityActivated = new GenericAbilityEvent(); 
        // private GenericAbilityEvent onGameplayAbilityCommitted = new GenericAbilityEvent();
        // private GenericAbilityEvent onGameplayAbilityEnded = new GenericAbilityEvent();

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

        // public void HandleGameplayEvent(GameplayTag EventTag, GameplayEventData Payload) {
            /**
             * TODO: Handle triggered abilities
             * Search component for all abilities that are automatically triggered from a gameplay event
             */

            // OnGameplayEvent.Invoke(EventTag, Payload);
        // }
    }

//----------------------------------------------------------------------------------------
    [System.Serializable]
    public class GenericGameplayEffectEvent : UnityEvent<GameplayEffect> {

    }
}
