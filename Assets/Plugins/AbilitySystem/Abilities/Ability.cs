using System.Collections.Generic;
using GameplayAbilitySystem.Effects;
using UnityEngine;
using System.Linq;

namespace GameplayAbilitySystem.Abilities 
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System/Ability")]
    /// <summary>
    /// Abilities represent "things" that players can cast, etc.
    /// E.g. a Ability might represent a fireball ability which the player casts and which damages a target
    /// </summary>
    public class Ability : ScriptableObject
    {
        public int Id => id;
        public Sprite Icon => icon;
        public Effect CostEffect => costEffect; // Cost of using this ability
        public List<Effect> CooldownEffects => cooldownEffects;     // Cooldowns associated with this ability
        public AbilityLogic AbilityLogic => abilityLogic; // Defines what the ability actually does

        [SerializeField] private int id = default;
        [SerializeField] private Sprite icon = default;
        // [SerializeField] private AbilityTagContainer tags = default;
        [SerializeField] private Effect costEffect = default;
        [SerializeField] private List<Effect> cooldownEffects = default;
        [SerializeField] private AbilityLogic abilityLogic = default;

        // [SerializeField] private List<GameplayEffect> _effectsToApplyOnExecution = new List<GameplayEffect>();
        // [SerializeField] private GenericAbilityEvent _onGameplayAbilityCommitted = new GenericAbilityEvent();
        // [SerializeField] private GenericAbilityEvent _onGameplayAbilityCancelled = new GenericAbilityEvent();
        // [SerializeField] private GenericAbilityEvent _onGameplayAbilityEnded = new GenericAbilityEvent();
        // public List<GameplayEffect> EffectsToApplyOnExecution => _effectsToApplyOnExecution;
        // public GenericAbilityEvent OnGameplayAbilityCommitted => _onGameplayAbilityCommitted;
        // public GenericAbilityEvent OnGameplayAbilityCancelled => _onGameplayAbilityCancelled;
        // public GenericAbilityEvent OnGameplayAbilityEnded => _onGameplayAbilityEnded;

        public bool IsActivatable(AbilitySystem instigator) 
        {
            // Player must be "Idle" to begin ability activation
            if (instigator.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != Animator.StringToHash("Base.Idle")) return false;
            return IsCostSatisfied(instigator) && !IsCooling(instigator) && IsTagsSatisfied(instigator);
        }

        public bool Commit(AbilitySystem instigator) 
        {
            Activate(instigator);
            // AbilitySystem.OnGameplayAbilityActivated.Invoke(this);
            ApplyCost(instigator);
            return true;
        }

        public void Activate(AbilitySystem instigator) 
        {
            AbilityLogic.Execute(instigator, this);
            ApplyCooldown(instigator);
        }

        private bool IsTagsSatisfied(AbilitySystem instigator) 
        {
            // Checks to make sure Source ability system doesn't have prohibited tags
            // var activeTags = abilitySystem.ActiveTags;
            bool hasActivationRequiredTags = true;
            bool hasActivationBlockedTags = false;
            // bool hasSourceRequiredTags = false;
            // bool hasSourceBlockedTags = false;

            // if (Tags.ActivationRequiredTags.Added.Count > 0) {
            //     hasActivationRequiredTags = !Tags.ActivationRequiredTags.Added.Except(activeTags).Any();
            // }
            // if (Tags.ActivationBlockedTags.Added.Count > 0) {
            //     hasActivationBlockedTags = activeTags.Any(x => Tags.ActivationBlockedTags.Added.Contains(x));
            // }
            // if (Tags.SourceRequiredTags.Added.Count > 0) {
            //     hasSourceRequiredTags = !Tags.SourceRequiredTags.Added.Except(activeTags).Any();
            // }
            // if (Tags.SourceBlockedTags.Added.Count > 0) {
            //     hasSourceBlockedTags = activeTags.Any(x => Tags.SourceBlockedTags.Added.Contains(x));
            // }
            return !hasActivationBlockedTags && hasActivationRequiredTags;
        }

        public void End(AbilitySystem instigator) 
        {
            // _onGameplayAbilityEnded.Invoke(this);

            // Ability finished.  Remove all listeners.
            // _onGameplayAbilityEnded.RemoveAllListeners();
            // TODO: Remove tags added by this ability
            // TODO: Cancel all tasks?
            // TODO: Remove gameplay cues
            // TODO: Cancel ability
            // TODO: Remove blocking/cancelling Gameplay Tags

            // Tell ability system ability has ended
            instigator.NotifyAbilityEnded(this);
        }

        /// <summary>
        /// Applies cooldown. Cooldown is applied even if the ability is already
        /// on cooldown
        /// </summary>
        protected void ApplyCooldown(AbilitySystem instigator) 
        {
            foreach (var cooldown in CooldownEffects) {
                instigator.ApplyEffectToTarget(id, cooldown, instigator);
            }
        }

        public CoolDownInfo CalculateCooldown(AbilitySystem instigator) 
        {
            CoolDownInfo info = new CoolDownInfo(isCooling: false);
            // Iterate through all gameplay effects on the ability system and find all effects which grant these cooldown tags
            EffectContext maxCDEffectContext = instigator.EffectsContainer
                                    .GetAllEffects()
                                    .Where(x => x.IsCoolDownOf(this))
                                    .DefaultIfEmpty()
                                    .OrderByDescending(x => x?.RemainingTime)
                                    .FirstOrDefault();

            if (maxCDEffectContext == null) {
                return info;
            }
            info.isCooling = true;
            info.elapsed = maxCDEffectContext.ElapsedTime;
            info.total = maxCDEffectContext.TotalTime;
            return info;
        }

        // Checks to see if the target GAS has the required cost resource to cast the ability
        private bool IsCostSatisfied(AbilitySystem instigator) 
        {
            // Check the modifiers on the ability cost GameEffect
            var modifiers = CostEffect.CalculateModifiers();
            var attributeModification = CostEffect.CalculateAttributes(
                    instigator, modifiers, operateOnCurrentValue: true);

            foreach (var attribute in attributeModification) {
                if (attribute.Value.newValue < 0) return false;
            }
            return true;
        }

        /// <summary>
        /// Applies the ability cost, decreasing the specified cost resource from the player.
        /// If player doesn't have the required resource, the resource goes to negative (or clamps to 0)
        /// </summary>
        private void ApplyCost(AbilitySystem instigator) 
        {
            var modifiers = CostEffect.CalculateModifiers();
            var attributeModification = CostEffect.CalculateAttributes(instigator, modifiers);
            CostEffect.ApplyInstantEffect(instigator);
        }

        // Checks to see if the ability is off cooldown
        private bool IsCooling(AbilitySystem instigator) 
        {
            CoolDownInfo info = CalculateCooldown(instigator);
            return info.isCooling;
        }

        public struct CoolDownInfo
        {
            public bool isCooling;
            public float elapsed;
            public float total;

            public CoolDownInfo(bool isCooling, float elapsed = 0f, float total = 0f)
            {
                this.isCooling = isCooling;
                this.elapsed = elapsed;
                this.total = total;
            }
        }
    }
}
