﻿using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GAS.Abilities;
using GAS.GameplayEffects;
using UnityEngine;
using GAS.Attributes;
using GAS.GameplayCues;
using UnityEngine.Events;
using GAS.Interfaces;

namespace GAS {

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
        public ActiveGameplayEffectsContainer ActiveGameplayEffectsContainer => activeGameplayEffectsContainer;
        public Transform Actor => transform;

        private GameplayEvent onGameplayEvent = new GameplayEvent();
        // private GenericAbilityEvent onGameplayAbilityActivated = new GenericAbilityEvent(); 
        // private GenericAbilityEvent onGameplayAbilityCommitted = new GenericAbilityEvent();
        // private GenericAbilityEvent onGameplayAbilityEnded = new GenericAbilityEvent();
        private GenericGameplayEffectEvent onEffectAdded = new GenericGameplayEffectEvent();
        private GenericGameplayEffectEvent onEffectRemoved = new GenericGameplayEffectEvent();
        private List<IGameplayAbility> runningAbilities = new List<IGameplayAbility>();
        private ActiveGameplayEffectsContainer activeGameplayEffectsContainer;


        private Animator animator;
        public Animator Animator => animator;

        public IEnumerable<GameplayTag> ActiveTags {
            get {
                return this.ActiveGameplayEffectsContainer
                            .ActiveEffectAttributeAggregator
                            .GetAllActiveEffects()
                            .SelectMany(x => x.Effect.GameplayEffectTags.GrantedTags.Added)
                            .Union(AbilityGrantedTags);
            }
        }

        private IEnumerable<GameplayTag> AbilityGrantedTags => this.runningAbilities.SelectMany(x => x.Tags.ActivationOwnedTags.Added);

        public IEnumerable<(GameplayTag Tag, ActiveGameplayEffectData GrantingEffect)> ActiveTagsByActiveGameplayEffect {
            get {
                var activeEffects = this.ActiveGameplayEffectsContainer
                            .ActiveEffectAttributeAggregator
                            .GetAllActiveEffects();

                if (activeEffects == null) return new List<(GameplayTag, ActiveGameplayEffectData)>();

                var activeEffectsTags = activeEffects.SelectMany(x =>
                     x.Effect.GrantedTags
                     .Select(y => (y, x)));

                return activeEffectsTags;
            }
        }

        public void Awake() {
            activeGameplayEffectsContainer = new ActiveGameplayEffectsContainer(this);
            animator = this.GetComponent<Animator>();
        }


        public void HandleGameplayEvent(GameplayTag EventTag, GameplayEventData Payload) {
            /**
             * TODO: Handle triggered abilities
             * Search component for all abilities that are automatically triggered from a gameplay event
             */

            OnGameplayEvent.Invoke(EventTag, Payload);
        }

        public void NotifyAbilityEnded(GameplayAbility ability) {
            runningAbilities.Remove(ability);
        }

        public bool TryActivateAbility(GameplayAbility Ability) {
            if (!this.CanActivateAbility(Ability)) return false;
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

        public async void ApplyBatchGameplayEffects(IEnumerable<(GameplayEffect Effect, AbilitySystemComponent Target, float Level)> BatchedGameplayEffects) {

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

        public Task<GameplayEffect> ApplyGameEffectToTarget(GameplayEffect Effect, AbilitySystemComponent Target, float Level = 0) {
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
                cue.HandleGameplayCue(Target.Actor.gameObject, new GameplayCueParameters(null, null, null), EGameplayCueEvent.OnActive);
            }

            return Task.FromResult(Effect);
        }

        public float GetNumericAttributeBase(AttributeType AttributeType) {
            var attributeSet = this.GetComponent<AttributeSet>();
            var attribute = attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == AttributeType);
            if (attribute == null) return 0;
            return attribute.BaseValue;
        }

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

    [System.Serializable]
    public class GenericGameplayEffectEvent : UnityEvent<GameplayEffect> {

    }
}
