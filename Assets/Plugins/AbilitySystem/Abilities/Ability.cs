using System.Collections.Generic;
using GameplayAbilitySystem.Effects;
using UnityEngine;
using System.Linq;

namespace GameplayAbilitySystem.Abilities 
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System/Ability")]
    public class Ability : ScriptableObject
    {
        public int Id => id;
        public Sprite Icon => icon;
        public Effect CostEffect => costEffect; 
        public List<Effect> CooldownEffects => cooldownEffects;
        public AbilityLogic AbilityLogic => abilityLogic;

        [SerializeField] private int id = default;
        [SerializeField] private Sprite icon = default;
        [SerializeField] private Effect costEffect = default;
        [SerializeField] private List<Effect> cooldownEffects = default;
        [SerializeField] private AbilityLogic abilityLogic = default;

        public bool IsActivatable(AbilitySystem instigator) 
        {
            // Player must be "Idle" to begin ability activation
            if (instigator.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != Animator.StringToHash("Base.Idle")) 
                return false;
            return IsCostSatisfied(instigator) && !IsCooling(instigator);
        }

        public bool Commit(AbilitySystem instigator) 
        {
            AbilityLogic.Execute(instigator, this);
            ApplyCooldown(instigator);
            ApplyCost(instigator);
            return true;
        }

        public void End(AbilitySystem instigator) 
        {
            // TODO: Cancel all tasks?
            // TODO: Cancel ability
            instigator.NotifyAbilityEnded(this);
        }

        private void ApplyCooldown(AbilitySystem instigator) 
        {
            foreach (var cooldown in CooldownEffects) {
                instigator.ApplyEffectToTarget(id, cooldown, instigator);
            }
        }

        public CoolDownInfo CalculateCooldown(AbilitySystem instigator) 
        {
            CoolDownInfo info = new CoolDownInfo(isCooling: false);

            EffectContext maxCDEffectContext = instigator.ActivedEffects
                                    .AllEffects
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

        private bool IsCostSatisfied(AbilitySystem instigator) 
        {
            //       attribute type         operation type      value
            Dictionary<string, Dictionary<OperationType, float>> modifiers = CostEffect.CalculateModifiers();

            List<AttributeModifyInfo> totalModifyInfo =
                    CostEffect.CalculateAttributes(instigator, modifiers, operateOnCurrentValue: true);

            foreach (var singleModifyInfo in totalModifyInfo) {
                if (singleModifyInfo.NewValue < 0) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Applies the ability cost, decreasing the specified cost resource from the player.
        /// If player doesn't have the required resource, the resource goes to negative (or clamps to 0)
        /// </summary>
        private void ApplyCost(AbilitySystem instigator) 
        {
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
