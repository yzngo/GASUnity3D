using System.Collections.Generic;
using GameplayAbilitySystem.Interfaces;
using GameplayAbilitySystem.Effects;
using UnityEngine;
using GameplayAbilitySystem.Abilities.AbilityActivations;
using System.Linq;

namespace GameplayAbilitySystem.Abilities 
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System/Ability")]
    public class GameplayAbility : ScriptableObject, IGameplayAbility 
    {
        public GameplayAbilityTags Tags => _tags;
        public GameplayEffect Cost => cost;
        public List<GameplayEffect> Cooldowns => _cooldownsToApply;
        public AbstractAbilityActivation AbilityLogic => _abilityLogic;

        [SerializeField] private GameplayAbilityTags _tags = new GameplayAbilityTags();
        [SerializeField] private GameplayEffect cost = default;
        [SerializeField] private List<GameplayEffect> _cooldownsToApply = new List<GameplayEffect>();
        [SerializeField] private AbstractAbilityActivation _abilityLogic = default;

        // [SerializeField] private List<GameplayEffect> _effectsToApplyOnExecution = new List<GameplayEffect>();
        // [SerializeField] private GenericAbilityEvent _onGameplayAbilityCommitted = new GenericAbilityEvent();
        // [SerializeField] private GenericAbilityEvent _onGameplayAbilityCancelled = new GenericAbilityEvent();
        // [SerializeField] private GenericAbilityEvent _onGameplayAbilityEnded = new GenericAbilityEvent();
        // public List<GameplayEffect> EffectsToApplyOnExecution => _effectsToApplyOnExecution;
        // public GenericAbilityEvent OnGameplayAbilityCommitted => _onGameplayAbilityCommitted;
        // public GenericAbilityEvent OnGameplayAbilityCancelled => _onGameplayAbilityCancelled;
        // public GenericAbilityEvent OnGameplayAbilityEnded => _onGameplayAbilityEnded;

        public bool IsAbilityActivatable(AbilitySystem abilitySystem) {
            // Player must be "Idle" to begin ability activation
            if (abilitySystem.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != Animator.StringToHash("Base.Idle")) return false;
            return CheckCost(abilitySystem) && AbilityOffCooldown(abilitySystem) && IsTagsSatisfied(abilitySystem);
        }

        public void ActivateAbility(AbilitySystem abilitySystem) {
            AbilityLogic.ActivateAbility(abilitySystem, this);
            ApplyCooldown(abilitySystem);
        }


        public bool CommitAbility(AbilitySystem abilitySystem) {
            ActivateAbility(abilitySystem);
            // AbilitySystem.OnGameplayAbilityActivated.Invoke(this);
            ApplyCost(abilitySystem);
            return true;
        }

        private bool IsTagsSatisfied(AbilitySystem abilitySystem) {
            // Checks to make sure Source ability system doesn't have prohibited tags
            var activeTags = abilitySystem.ActiveTags;
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



        public void EndAbility(AbilitySystem abilitySystem) {
            // _onGameplayAbilityEnded.Invoke(this);

            // Ability finished.  Remove all listeners.
            // _onGameplayAbilityEnded.RemoveAllListeners();
            // TODO: Remove tags added by this ability
            // TODO: Cancel all tasks?
            // TODO: Remove gameplay cues
            // TODO: Cancel ability
            // TODO: Remove blocking/cancelling Gameplay Tags

            // Tell ability system ability has ended
            abilitySystem.NotifyAbilityEnded(this);
        }

        /// <summary>
        /// Applies cooldown. Cooldown is applied even if the ability is already
        /// on cooldown
        /// </summary>
        protected void ApplyCooldown(AbilitySystem abilitySystem) {
            foreach (var cooldown in Cooldowns) {
                abilitySystem.ApplyEffectToTarget(cooldown, abilitySystem);
            }
        }

        public CoolDownInfo CalculateCooldown(AbilitySystem abilitySystem) {

            CoolDownInfo info = new CoolDownInfo();
            List<GameplayTag> cooldownTags = Tags.CooldownTags.Added;
            // Iterate through all gameplay effects on the ability system and find all effects which grant these cooldown tags
            ActivedEffectData maxCooldownEffect = abilitySystem.ActiveEffectsContainer
                                    .ActiveEffectAttributeAggregator
                                    .GetAllActiveEffects()
                                    .Where(x => x.Effect.GrantedTags.Intersect(cooldownTags).Any())
                                    .DefaultIfEmpty()
                                    .OrderByDescending(x => x?.CooldownTimeRemaining)
                                    .FirstOrDefault();

            if (maxCooldownEffect == null) {
                return info;
            }
            info.elapsed = maxCooldownEffect.CooldownTimeElapsed;
            info.total = maxCooldownEffect.CooldownTimeTotal;
            return info;
        }

        // Checks to see if the target GAS has the required cost resource to cast the ability
        private bool CheckCost(AbilitySystem abilitySystem) {
            // Check the modifiers on the ability cost GameEffect
            var modifiers = Cost.CalculateModifierEffect();
            var attributeModification = Cost.CalculateAttributeModification(
                    abilitySystem, modifiers, operateOnCurrentValue: true);

            foreach (var attribute in attributeModification) {
                if (attribute.Value.NewAttribueValue < 0) return false;
            }
            return true;
        }

        /// <summary>
        /// Applies the ability cost, decreasing the specified cost resource from the player.
        /// If player doesn't have the required resource, the resource goes to negative (or clamps to 0)
        /// </summary>
        private void ApplyCost(AbilitySystem abilitySystem) {
            var modifiers = Cost.CalculateModifierEffect();
            var attributeModification = Cost.CalculateAttributeModification(abilitySystem, modifiers);
            Cost.ApplyInstantEffect(abilitySystem);
        }

        // Checks to see if the GAS is off cooldown
        private bool AbilityOffCooldown(AbilitySystem abilitySystem) {
            CoolDownInfo info = CalculateCooldown(abilitySystem);
            // (var elapsed, var total) = CalculateCooldown(abilitySystem);
            return info.total == 0f;
        }
    }
}
