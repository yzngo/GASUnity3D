using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GameplayAbilitySystem
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System/Ability")]
    public class Ability : ScriptableObject
    {
        public string Id => id;
        public Effect CostEffect => costEffect;
        public List<Effect> CooldownEffects => cooldownEffects;
        public AbilityLogic AbilityLogic => abilityLogic;

        [SerializeField] private string id = default;
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
                                    .Where(x => x.IsCoolDownEffectOf(this))
                                    .DefaultIfEmpty()
                                    .OrderByDescending(x => x?.RemainingDuration)
                                    .FirstOrDefault();
            if (maxCDEffectContext == null) {
                return info;
            }
            info.isCooling = true;
            info.elapsed = maxCDEffectContext.ElapsedDuration;
            info.total = maxCDEffectContext.TotalDuration;
            return info;
        }

        private bool IsCostSatisfied(AbilitySystem instigator) 
        {
            //       attribute type         operation type      value
            Dictionary<string, Dictionary<OperationType, float>> modifiers = CostEffect.GetAllOperation();

            List<AttributeModifyInfo> totalModifyInfo =
                    CostEffect.GetAllModifyInfo(instigator, modifiers, operateOnCurrentValue: true);

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
            CostEffect.InstantApplyTo(instigator);
        }

        // Checks to see if the ability is off cooldown
        private bool IsCooling(AbilitySystem instigator) 
        {
            CoolDownInfo info = CalculateCooldown(instigator);
            return info.isCooling;
        }

        private static Dictionary<string, Ability> abilities = new Dictionary<string, Ability>();

        public static Ability Get(string abilityId)
        {
            if (!abilities.TryGetValue(abilityId, out var ability)) {
                ability = ScriptableObject.CreateInstance(typeof(Ability)) as Ability;
                ability.id = abilityId;
                ability.costEffect = Effect.Get(EffectConfigs.GetCostConfig(abilityId));
                ability.cooldownEffects = new List<Effect>() {
                    Effect.Get(EffectConfigs.GetCoolDownConfig(abilityId)),
                    Effect.Get(EffectConfigs.GetGlobalCoolDownConfig())
                };
                if (abilityId == ID.ability_fire) {
                    ability.abilityLogic = TrackingAttack.Get(abilityId);
                } else {
                    ability.abilityLogic = ApplyEffect.Get(abilityId);
                }
            }
            return ability;
        }
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
