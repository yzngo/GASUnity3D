using System.Collections.Generic;
using AbilitySystem.Interfaces;
using AbilitySystem.GameplayEffects;
using UnityEngine;
using AbilitySystem.Abilities.AbilityActivations;
using System.Linq;

namespace AbilitySystem.Abilities {
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

        public GameplayAbilityTags Tags => _tags;
        public GameplayCost Cost => _gameplayCost;
        public List<GameplayEffect> Cooldowns => _cooldownsToApply;
        /// Defines what the ability actually does
        public AbstractAbilityActivation AbilityLogic => _abilityLogic;

        // public List<GameplayEffect> EffectsToApplyOnExecution => _effectsToApplyOnExecution;
        // public GenericAbilityEvent OnGameplayAbilityCommitted => _onGameplayAbilityCommitted;
        // public GenericAbilityEvent OnGameplayAbilityCancelled => _onGameplayAbilityCancelled;
        // public GenericAbilityEvent OnGameplayAbilityEnded => _onGameplayAbilityEnded;

        public bool IsAbilityActivatable(AbilitySystemComponent ASC) {
            // Player must be "Idle" to begin ability activation
            if (ASC.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != Animator.StringToHash("Base.Idle")) return false;
            return CheckCost(ASC) && AbilityOffCooldown(ASC) && IsTagsSatisfied(ASC);
        }

        public void ActivateAbility(AbilitySystemComponent ASC) {
            _abilityLogic.ActivateAbility(ASC, this);
            ApplyCooldown(ASC);
        }


        public bool CommitAbility(AbilitySystemComponent ASC) {
            ActivateAbility(ASC);
            // AbilitySystem.OnGameplayAbilityActivated.Invoke(this);
            ApplyCost(ASC);
            return true;
        }

        private bool IsTagsSatisfied(AbilitySystemComponent ASC) {
            // Checks to make sure Source ability system doesn't have prohibited tags
            var activeTags = ASC.ActiveTags;
            bool hasActivationRequiredTags = true;
            bool hasActivationBlockedTags = false;
            bool hasSourceRequiredTags = false;
            bool hasSourceBlockedTags = false;

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



        public void EndAbility(AbilitySystemComponent ASC) {
            // _onGameplayAbilityEnded.Invoke(this);

            // Ability finished.  Remove all listeners.
            // _onGameplayAbilityEnded.RemoveAllListeners();
            // TODO: Remove tags added by this ability
            // TODO: Cancel all tasks?
            // TODO: Remove gameplay cues
            // TODO: Cancel ability
            // TODO: Remove blocking/cancelling Gameplay Tags

            // Tell ability system ability has ended
            ASC.NotifyAbilityEnded(this);
        }

        /// <summary>
        /// Applies cooldown. Cooldown is applied even if the ability is already
        /// on cooldown
        /// </summary>
        protected void ApplyCooldown(AbilitySystemComponent ASC) {
            foreach (var cooldown in Cooldowns) {
                ASC.ApplyEffectToTarget(cooldown, ASC);
            }
        }

        public (float CooldownElapsed, float CooldownTotal) CalculateCooldown(AbilitySystemComponent ASC) {
            List<GameplayTag> cooldownTags = Tags.CooldownTags.Added;

            // Iterate through all gameplay effects on the ability system and find all effects which grant these cooldown tags
            ActiveGameplayEffectData dominantCooldownEffect = ASC.ActiveEffectsContainer
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

        // Checks to see if the target GAS has the required cost resource to cast the ability
        private bool CheckCost(AbilitySystemComponent ASC) {
            // Check the modifiers on the ability cost GameEffect
            var modifiers = Cost.CostGameplayEffect.CalculateModifierEffect();
            var attributeModification = Cost.CostGameplayEffect.CalculateAttributeModification(
                    ASC, modifiers, operateOnCurrentValue: true);

            foreach (var attribute in attributeModification) {
                if (attribute.Value.NewAttribueValue < 0) return false;
            }
            return true;
        }

        /// <summary>
        /// Applies the ability cost, decreasing the specified cost resource from the player.
        /// If player doesn't have the required resource, the resource goes to negative (or clamps to 0)
        /// </summary>
        private void ApplyCost(AbilitySystemComponent ASC) {
            var modifiers = Cost.CostGameplayEffect.CalculateModifierEffect();
            var attributeModification = Cost.CostGameplayEffect.CalculateAttributeModification(ASC, modifiers);
            Cost.CostGameplayEffect.ApplyInstantEffect(ASC);
        }

        // Checks to see if the GAS is off cooldown
        private bool AbilityOffCooldown(AbilitySystemComponent ASC) {
            (var elapsed, var total) = CalculateCooldown(ASC);
            return total == 0f;
        }
    }
}
