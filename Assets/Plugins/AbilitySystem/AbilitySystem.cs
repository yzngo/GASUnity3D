﻿using System.Linq;
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

    /// The AbilitySytem is the primary component. Every game object 
    /// that needs to participate with the GAS needs to have this component attached.
    [AddComponentMenu("Ability System/Ability System")]
    [RequireComponent(typeof(AttributeSet))]
    public class AbilitySystem : MonoBehaviour {

        // 自己身上作为目标的点
        [SerializeField] private Transform targetPoint = default;
        public Transform TargetPoint => targetPoint;

        // Called when a AbilityEvent is executed
        private AbilityEvent onAbilityEvent = new AbilityEvent();
        public AbilityEvent OnAbilityEvent => onAbilityEvent;

        private List<Ability> runningAbilities = new List<Ability>();

        // Lists all active Effect
        private EffectsContainer effectsContainer;
        public EffectsContainer EffectsContainer => effectsContainer;

        private Animator animator;
        public Animator Animator => animator;

        private AttributeSet attributeSet;

        // public IEnumerable<GameplayTag> ActiveTags =>
        //         EffectsContainer
        //                     .effectsModifyAggregator
        //                     .GetAllEffects()
        //                     .SelectMany(x => x.Effect.EffectTags.GrantedToASCTags.Added)
        //                     .Union(runningAbilities.SelectMany(x => x.Tags.ActivationOwnedTags.Added));

        // private IEnumerable<GameplayTag> AbilityGrantedTags => 

        public void Awake() {
            effectsContainer = new EffectsContainer(this);
            animator = GetComponent<Animator>();
            attributeSet = GetComponent<AttributeSet>();
        }

        // Checks to see if the ability can be activated
        // DO NOT execute the ability
        // 激活之后后续流程交给ability
        public bool CanActivateAbility(Ability ability) {
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
        public bool TryActivateAbility(Ability ability, AbilitySystem target = null) {
            if (!CanActivateAbility(ability)) {
                return false;
            }
            // 一个技能的生命周期是从[释放开始]到[释放结束], 之后的工作交给effect
            // 技能仅仅是一个时序流程
            runningAbilities.Add(ability);
            ability.CommitAbility(this);

            GameplayTag abilityTag = ability.Tags.AbilityTags.Count > 0 ? ability.Tags.AbilityTags[0] : new GameplayTag();
            var data = new AbilityEventData();
            data.Target = target;
            onAbilityEvent?.Invoke(abilityTag, data);
            return true;
        }

        // Notifies this AbilitySystem that the specified ability has ended
        public void NotifyAbilityEnded(Ability ability) {
            runningAbilities.Remove(ability);
        }
        
        // Apply batched effect.
        // This can be useful if e.g. an ability applies a number of effects,
        // some with instant modifiers, and some with infinite or duration modifiers.
        // By batching these effects, we can ensure that all these effect happen 
        // with reference to the same base attribute value.
        public void ApplyBatchGameplayEffects(IEnumerable<(Effect Effect, AbilitySystem Target, float Level)> batchedGameplayEffects) {

            foreach(var effect in batchedGameplayEffects) {
                ApplyEffectToTarget(effect.Effect, effect.Target, effect.Level);
            }
            // var instantEffects = batchedGameplayEffects.Where(x => x.Effect.Policy.DurationPolicy == DurationPolicy.Instant);
            // var durationalEffects = batchedGameplayEffects.Where(
            //     x =>
            //         x.Effect.Policy.DurationPolicy == DurationPolicy.Duration ||
            //         x.Effect.Policy.DurationPolicy == DurationPolicy.Infinite
            //         );
            // Apply instant effects
            // foreach (var effect in instantEffects) {
            //     ApplyEffectToTarget(effect.Effect, effect.Target);
            // }
            // Apply durational effects
            // foreach (var effect in durationalEffects) {
            //     ApplyEffectToTarget(effect.Effect, effect.Target);
            // }
        }
        
        // effect也是ablity system管理
        // Apply a effect to the target
        // The overall effect may be modulated by the Level.
        // level -> maybe used to affect the "strength" of the effect
        public void ApplyEffectToTarget(Effect appliedEffect, AbilitySystem target, float level = 0) {
            // Check to make sure all the attributes being modified by this effect exist on the target
            foreach(var modifiers in appliedEffect.Policy.Modifiers) {
                if (!target.IsAttributeExist(modifiers.AttributeType)) {
                    return ;
                }
            }
            // TODO: Get list of tags owned by target
            // TODO: Check for immunity tags, and don't apply effect if target is immune (and also add Immunity Tags container to IGameplayEffect)
            // TODO: Check to make sure Application Tag Requirements are met (i.e. target has all the required tags, and does not contain any prohibited tags )
            if (!appliedEffect.IsTagsRequiredMatch(target)) {
                return ;
            }

            // Handling Instant effects is different to handling HasDuration and Infinite effects, instant effect modify the base value
            if (appliedEffect.Policy.DurationPolicy == DurationPolicy.Instant) {
                appliedEffect.ApplyInstantEffect(target);
            } else {
                // Durational effect require attention to many more things than instant effects
                // Such as stacking and effect durations
                // Durational effect modify the current value
                var effectContext = new EffectContext(appliedEffect, this, target);
                target.EffectsContainer.ApplyDurationalEffect(effectContext);
            }

            // Remove all effects which have tags defined as "Be Removed Effects Tags". 
            // We do this by setting the expiry time on the effect to make it end prematurely
            // This is accomplished by finding all effects which grant these tags, and then adjusting start time
            var tagsToRemove = appliedEffect.EffectTags.RemovedEffectsTags.Removed;
            var beRemovedEffects = target.GetActiveEffectsTags()
                                    .Where(x => tagsToRemove.Any(y => x.Tag == y.Tag))
                                    .Join(tagsToRemove, x => x.Tag, x => x.Tag, (x, y) => new { Tag = x.Tag, effectContext = x.GrantingEffect, StacksToRemove = y.StacksToRemove })
                                    .OrderBy(x => x.effectContext.CooldownTimeRemaining);

            Dictionary<Effect, int> stacks = new Dictionary<Effect, int>();

            foreach (var beRemovedEffect in beRemovedEffects) {
                var effect = beRemovedEffect.effectContext.Effect;
                if (!stacks.ContainsKey(effect)) {
                    stacks.Add(effect, 0);
                }

                if (beRemovedEffect.StacksToRemove == 0 || stacks[effect] < beRemovedEffect.StacksToRemove) {
                    beRemovedEffect.effectContext.ForceEndEffect();
                }
                stacks[effect]++;
            }

            // Execute gameplay cue
            var gameplayCues = appliedEffect.GameplayCues;
            for (var i = 0; i < gameplayCues.Count; i++) {
                var cue = gameplayCues[i];
                cue.HandleCue(target, CueEventMomentType.OnActive);
            }
        }

        public IEnumerable<(GameplayTag Tag, EffectContext GrantingEffect)> GetActiveEffectsTags()
        {
            var activeEffects = EffectsContainer.effectsModifyAggregator.GetAllEffects();
            if (activeEffects == null) 
                return new List<(GameplayTag, EffectContext)>();
            return activeEffects.SelectMany(x => x.Effect.GrantedTags.Select(y => (y, x)));
        }


// attribute -----------------------------------------------------------------------------
        public bool IsAttributeExist(AttributeType type) => attributeSet.Attributes.Exists( x => x.AttributeType == type);
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
    public class GenericGameplayEffectEvent : UnityEvent<Effect> {

    }
}