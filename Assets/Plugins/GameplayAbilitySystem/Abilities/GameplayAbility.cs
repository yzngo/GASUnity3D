using System.Collections.Generic;
using GAS.Interfaces;
using GAS.GameplayEffects;
using UnityEngine;
using GAS.Abilities.AbilityActivations;
using System.Linq;

namespace GAS.Abilities {
    /// <inheritdoc />
    [AddComponentMenu("Ability System/Ability")]
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System/Ability")]
    public class GameplayAbility : ScriptableObject, IGameplayAbility {

        [SerializeField] private GameplayAbilityTags _tags = new GameplayAbilityTags();
        [SerializeField] private GameplayCost _gameplayCost = new GameplayCost();
        [SerializeField] private List<GameplayEffect> _cooldownsToApply = new List<GameplayEffect>();
        // [SerializeField] private List<GameplayEffect> _effectsToApplyOnExecution = new List<GameplayEffect>();
        // [SerializeField] private GenericAbilityEvent _onGameplayAbilityCommitted = new GenericAbilityEvent();
        // [SerializeField] private GenericAbilityEvent _onGameplayAbilityCancelled = new GenericAbilityEvent();
        // [SerializeField] private GenericAbilityEvent _onGameplayAbilityEnded = new GenericAbilityEvent();
        [SerializeField] private AbstractAbilityActivation _abilityLogic = null;

        public IAbilityTags Tags => _tags;
        public IGameplayCost Cost => _gameplayCost;
        public List<GameplayEffect> Cooldowns => _cooldownsToApply;
        // public List<GameplayEffect> EffectsToApplyOnExecution => _effectsToApplyOnExecution;
        // public GenericAbilityEvent OnGameplayAbilityCommitted => _onGameplayAbilityCommitted;
        // public GenericAbilityEvent OnGameplayAbilityCancelled => _onGameplayAbilityCancelled;
        // public GenericAbilityEvent OnGameplayAbilityEnded => _onGameplayAbilityEnded;
        public AbstractAbilityActivation AbilityLogic => _abilityLogic;

        public virtual void ActivateAbility(IGameplayAbilitySystem AbilitySystem) {
            _abilityLogic.ActivateAbility(AbilitySystem, this);
            ApplyCooldown(AbilitySystem);
        }

        public virtual bool IsAbilityActivatable(IGameplayAbilitySystem AbilitySystem) {
            // Player must be "Idle" to begin ability activation
            if (AbilitySystem.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != Animator.StringToHash("Base.Idle")) return false;
            return PlayerHasResourceToCast(AbilitySystem) && AbilityOffCooldown(AbilitySystem) && TagRequirementsMet(AbilitySystem);
        }

        private bool TagRequirementsMet(IGameplayAbilitySystem AbilitySystem) {
            // Checks to make sure Source ability system doesn't have prohibited tags
            var activeTags = AbilitySystem.ActiveTags;
            var hasActivationRequiredTags = true;
            var hasActivationBlockedTags = false;
            var hasSourceRequiredTags = false;
            var hasSourceBlockedTags = false;


            if (Tags.ActivationRequiredTags.Added.Count > 0) {
                hasActivationRequiredTags = !Tags.ActivationRequiredTags.Added.Except(activeTags).Any();
            }

            if (Tags.ActivationBlockedTags.Added.Count > 0) {
                hasActivationBlockedTags = activeTags.Any(x => Tags.ActivationBlockedTags.Added.Contains(x));
            }

            if (Tags.SourceRequiredTags.Added.Count > 0) {
                hasSourceRequiredTags = !Tags.SourceRequiredTags.Added.Except(activeTags).Any();
            }

            if (Tags.SourceBlockedTags.Added.Count > 0) {
                hasSourceBlockedTags = activeTags.Any(x => Tags.SourceBlockedTags.Added.Contains(x));
            }

            return !hasActivationBlockedTags && hasActivationRequiredTags;
        }

        /// <summary>
        /// Applies the ability cost, decreasing the specified cost resource from the player.
        /// If player doesn't have the required resource, the resource goes to negative (or clamps to 0)
        /// </summary>
        protected void ApplyCost(IGameplayAbilitySystem AbilitySystem) {
            var modifiers = Cost.CostGameplayEffect.CalculateModifierEffect();
            var attributeModification = Cost.CostGameplayEffect.CalculateAttributeModification(AbilitySystem, modifiers);
            Cost.CostGameplayEffect.ApplyInstantEffect(AbilitySystem);
        }

        /// <summary>
        /// Applies cooldown.  Cooldown is applied even if the  ability is already
        /// on cooldown
        /// </summary>
        protected void ApplyCooldown(IGameplayAbilitySystem abilitySystem) {
            foreach (var cooldownEffect in Cooldowns) {
                abilitySystem.ApplyGameEffectToTarget(cooldownEffect, abilitySystem);
            }
        }

        public void EndAbility(IGameplayAbilitySystem AbilitySystem) {
            // _onGameplayAbilityEnded.Invoke(this);

            // Ability finished.  Remove all listeners.
            // _onGameplayAbilityEnded.RemoveAllListeners();

            // TODO: Remove tags added by this ability

            // TODO: Cancel all tasks?

            // TODO: Remove gameplay cues

            // TODO: Cancel ability

            // TODO: Remove blocking/cancelling Gameplay Tags

            // Tell ability system ability has ended
            AbilitySystem.NotifyAbilityEnded(this);
        }

        public bool PlayerHasResourceToCast(IGameplayAbilitySystem AbilitySystem) {
            // Check the modifiers on the ability cost GameEffect
            var modifiers = Cost.CostGameplayEffect.CalculateModifierEffect();
            var attributeModification = Cost.CostGameplayEffect.CalculateAttributeModification(
                    AbilitySystem, modifiers, operateOnCurrentValue: true);

            foreach (var attribute in attributeModification) {
                if (attribute.Value.NewAttribueValue < 0) return false;
            }
            return true;
        }

        public bool CommitAbility(IGameplayAbilitySystem AbilitySystem) {
            ActivateAbility(AbilitySystem);
            AbilitySystem.OnGameplayAbilityActivated.Invoke(this);
            ApplyCost(AbilitySystem);
            return true;
        }

        public bool AbilityOffCooldown(IGameplayAbilitySystem AbilitySystem) {
            (var elapsed, var total) = CalculateCooldown(AbilitySystem);
            return total == 0f;
        }

        public List<GameplayTag> GetAbilityCooldownTags() {
            return _tags.CooldownTags.Added;
        }

        public (float CooldownElapsed, float CooldownTotal) CalculateCooldown(IGameplayAbilitySystem AbilitySystem) {
            var cooldownTags = GetAbilityCooldownTags();

            // Iterate through all gameplay effects on the ability system and find all effects which grant these cooldown tags
            var dominantCooldownEffect = AbilitySystem.ActiveGameplayEffectsContainer
                                    .ActiveEffectAttributeAggregator
                                    .GetAllActiveEffects()
                                    .Where(x => x.Effect.GrantedTags.Intersect(cooldownTags).Any())
                                    .DefaultIfEmpty()
                                    .OrderByDescending(x => x?.CooldownTimeRemaining)
                                    .FirstOrDefault();

            if (dominantCooldownEffect == null) {
                return (0f, 0f);
            }
            return (dominantCooldownEffect.CooldownTimeElapsed, dominantCooldownEffect.CooldownTimeTotal);
        }

    }
}
