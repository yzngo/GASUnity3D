using System.Linq;
using System.Runtime.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GAS.ExtensionMethods;
using GAS.Abilities;
using GAS.GameplayEffects;
using GAS.Interfaces;
using UnityEngine;
using GAS.Attributes;
using UnityEngine.Events;
using GAS.GameplayCues;

namespace GAS {
    /// <inheritdoc />
    [AddComponentMenu("Gameplay Ability System/Ability System")]
    public class AbilitySystemComponent : MonoBehaviour, IGameplayAbilitySystem {
        public Transform TargettingLocation;

        // [HideInInspector,SerializeField] private GameplayEvent onGameplayEvent = new GameplayEvent();
        // [HideInInspector,SerializeField] private GenericAbilityEvent onGameplayAbilityCommitted = new GenericAbilityEvent();
        // [HideInInspector,SerializeField] private GenericAbilityEvent onGameplayAbilityActivated = new GenericAbilityEvent(); 
        // [HideInInspector,SerializeField] private GenericAbilityEvent onGameplayAbilityEnded = new GenericAbilityEvent();
        // [HideInInspector,SerializeField] private GenericGameplayEffectEvent onEffectAdded = new GenericGameplayEffectEvent();
        // [HideInInspector,SerializeField] private GenericGameplayEffectEvent onEffectRemoved = new GenericGameplayEffectEvent();
        
        private GameplayEvent onGameplayEvent = new GameplayEvent();
        private GenericAbilityEvent onGameplayAbilityCommitted = new GenericAbilityEvent();
        private GenericAbilityEvent onGameplayAbilityActivated = new GenericAbilityEvent(); 
        private GenericAbilityEvent onGameplayAbilityEnded = new GenericAbilityEvent();
        private GenericGameplayEffectEvent onEffectAdded = new GenericGameplayEffectEvent();
        private GenericGameplayEffectEvent onEffectRemoved = new GenericGameplayEffectEvent();

        private List<IGameplayAbility> runningAbilities = new List<IGameplayAbility>();
        private ActiveGameplayEffectsContainer activeGameplayEffectsContainer;

        /// <inheritdoc />
        public GameplayEvent OnGameplayEvent => onGameplayEvent;

        /// <inheritdoc />
        public GenericAbilityEvent OnGameplayAbilityCommitted => onGameplayAbilityCommitted;
        /// <inheritdoc /> 
        public GenericAbilityEvent OnGameplayAbilityActivated => onGameplayAbilityActivated;
        /// <inheritdoc />
        public GenericAbilityEvent OnGameplayAbilityEnded => onGameplayAbilityEnded;

        /// <inheritdoc />
        public GenericGameplayEffectEvent OnEffectAdded => onEffectAdded;
        /// <inheritdoc />
        public GenericGameplayEffectEvent OnEffectRemoved => onEffectRemoved;

        /// <inheritdoc />
        public List<IGameplayAbility> RunningAbilities => runningAbilities;

        /// <inheritdoc />
        public ActiveGameplayEffectsContainer ActiveGameplayEffectsContainer => activeGameplayEffectsContainer;


        private Animator animator;

        public Animator Animator => animator;

        public IEnumerable<GameplayTag> ActiveTags {
            get {
                return this.ActiveGameplayEffectsContainer
                            .ActiveEffectAttributeAggregator
                            .GetActiveEffects()
                            .SelectMany(x => x.Effect.GameplayEffectTags.GrantedTags.Added)
                            .Union(AbilityGrantedTags);
            }
        }

        private IEnumerable<GameplayTag> AbilityGrantedTags => this.runningAbilities.SelectMany(x => x.Tags.ActivationOwnedTags.Added);

        public IEnumerable<(GameplayTag Tag, ActiveGameplayEffectData GrantingEffect)> ActiveTagsByActiveGameplayEffect {
            get {
                var activeEffects = this.ActiveGameplayEffectsContainer
                            .ActiveEffectAttributeAggregator
                            .GetActiveEffects();

                if (activeEffects == null) return new List<(GameplayTag, ActiveGameplayEffectData)>();

                var activeEffectsTags = activeEffects.SelectMany(x =>
                     x.Effect.GrantedTags
                     .Select(y => (y, x)));

                return activeEffectsTags;
            }
        }

        public void Awake() {
            this.activeGameplayEffectsContainer = new ActiveGameplayEffectsContainer(this);
            this.animator = this.GetComponent<Animator>();
        }

        /// <inheritdoc />
        public Transform GetActor() {
            return this.transform;
        }

        void Update() {

        }



        /// <inheritdoc />
        public void HandleGameplayEvent(GameplayTag EventTag, GameplayEventData Payload) {
            /**
             * TODO: Handle triggered abilities
             * Search component for all abilities that are automatically triggered from a gameplay event
             */

            OnGameplayEvent.Invoke(EventTag, Payload);
        }

        /// <inheritdoc />
        public void NotifyAbilityEnded(GameplayAbility ability) {
            runningAbilities.Remove(ability);
        }

        /// <inheritdoc />
        public bool TryActivateAbility(GameplayAbility Ability) {
            if (!this.CanActivateAbility(Ability)) return false;
            if (!Ability.IsAbilityActivatable(this)) return false;
            runningAbilities.Add(Ability);
            Ability.CommitAbility(this);

            return true;
        }

        /// <inheritdoc />
        public bool CanActivateAbility(IGameplayAbility Ability) {
            // Check if this ability is already active on this ASC
            if (runningAbilities.Contains(Ability)) {
                return false;
            }


            return true;
        }

        public async void ApplyBatchGameplayEffects(IEnumerable<(GameplayEffect Effect, IGameplayAbilitySystem Target, float Level)> BatchedGameplayEffects) {

            var instantEffects = BatchedGameplayEffects.Where(x => x.Effect.GameplayEffectPolicy.DurationPolicy == DurationPolicy.Instant);
            var durationalEffects = BatchedGameplayEffects.Where(
                x =>
                    x.Effect.GameplayEffectPolicy.DurationPolicy == DurationPolicy.Duration ||
                    x.Effect.GameplayEffectPolicy.DurationPolicy == DurationPolicy.Infinite
                    );

            // Apply instant effects
            foreach (var item in instantEffects) {
                if (await ApplyGameEffectToTarget(item.Effect, item.Target)) {
                    // item.Target.AddGameplayEffectToActiveList(Effect);

                }
            }

            // Apply durational effects
            foreach (var effect in durationalEffects) {
                if (await ApplyGameEffectToTarget(effect.Effect, effect.Target)) {

                }
            }

        }

        /// <inheritdoc />
        public Task<GameplayEffect> ApplyGameEffectToTarget(GameplayEffect Effect, IGameplayAbilitySystem Target, float Level = 0) {
            // TODO: Check to make sure all the attributes being modified by this gameplay effect exist on the target

            // TODO: Get list of tags owned by target

            // TODO: Check for immunity tags, and don't apply gameplay effect if target is immune (and also add Immunity Tags container to IGameplayEffect)

            // TODO: Check to make sure Application Tag Requirements are met (i.e. target has all the required tags, and does not contain any prohibited tags )
            if (!Effect.ApplicationTagRequirementMet(Target)) {
                return null;
            }

            // If this is a non-instant gameplay effect (i.e. it will modify the current value, not the base value)

            // If this is an instant gameplay effect (i.e. it will modify the base value)

            // Handling Instant effects is different to handling HasDuration and Infinite effects
            if (Effect.GameplayEffectPolicy.DurationPolicy == DurationPolicy.Instant) {
                Effect.ApplyInstantEffect(Target);
            } else {
                // Durational effects require attention to many more things than instant effects
                // Such as stacking and effect durations
                var EffectData = new ActiveGameplayEffectData(Effect, this, Target);
                _ = Target.ActiveGameplayEffectsContainer.ApplyGameEffect(EffectData);
            }

            // Remove all effects which have tags defined as "Remove Gameplay Effects With Tag". 
            // We do this by setting the expiry time on the effect to make it end prematurely
            // This is accomplished by finding all effects which grant these tags, and then adjusting start time
            var tagsToRemove = Effect.GameplayEffectTags.RemoveGameplayEffectsWithTag.Added;
            var activeGEs = Target.ActiveTagsByActiveGameplayEffect
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


            var gameplayCues = Effect.GameplayCues;
            // Execute gameplay cue
            for (var i = 0; i < gameplayCues.Count; i++) {
                var cue = gameplayCues[i];
                cue.HandleGameplayCue(Target.GetActor().gameObject, new GameplayCueParameters(null, null, null), EGameplayCueEvent.OnActive);
            }

            return Task.FromResult(Effect);
        }


        /// <inheritdoc />
        public float GetNumericAttributeBase(AttributeType AttributeType) {
            var attributeSet = this.GetComponent<AttributeSet>();
            var attribute = attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType);
            if (attribute == null) return 0;
            return attribute.BaseValue;
        }

        /// <inheritdoc />
        public float GetNumericAttributeCurrent(AttributeType AttributeType) {
            var attributeSet = this.GetComponent<AttributeSet>();
            return attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType).CurrentValue;
        }

        public void SetNumericAttributeBase(AttributeType AttributeType, float modifier) {
            var attributeSet = this.GetComponent<AttributeSet>();
            var attribute = attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType);
            var newValue = modifier;
            attribute.SetAttributeBaseValue(attributeSet, ref newValue);
        }

        public void SetNumericAttributeCurrent(AttributeType AttributeType, float NewValue) {
            var attributeSet = this.GetComponent<AttributeSet>();
            var attribute = attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType);
            attribute.SetAttributeCurrentValue(attributeSet, ref NewValue);
        }

    }




}
