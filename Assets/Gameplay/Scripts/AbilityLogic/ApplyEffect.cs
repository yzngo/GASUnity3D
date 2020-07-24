using UnityEngine;
using System.Collections.Generic;

namespace GameplayAbilitySystem
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System/Ability Logic/Instant Attack")]
    public class ApplyEffect : AbilityLogic 
    {
        [SerializeField] private bool waitForCastingAnimationComplete = false;
        [SerializeField] private Effect appliedEffectAfterComplete = default;

        public void SetData(bool wait, Effect effect)
        {
            waitForCastingAnimationComplete = wait;
            appliedEffectAfterComplete = effect;
        }

        public override async void Execute(AbilitySystem instigator, Ability ability) 
        {
            Animator animator = instigator.Animator;

            
            AbilityEventData abilityEventData = await instigator.OnAbilityStart.WaitForEvent(
                (eventData) => eventData.abilityId == ability.Id
            );

            animator.SetTrigger(AnimParams.Do_Magic);
            animator.SetTrigger(AnimParams.Execute_Magic_2);

            if ( waitForCastingAnimationComplete == true) {
                await instigator.OnAnimEvent.WaitForEvent( (x) => x == AnimEventKey.CastingComplete);
            }

            instigator.ApplyEffectToTarget(ability.Id, appliedEffectAfterComplete, abilityEventData.target);

            ActorFSMBehaviour fsmBehaviour = animator.GetBehaviour<ActorFSMBehaviour>();
            await fsmBehaviour.StateEnter.WaitForEvent(
                (_, stateInfo, _1) => stateInfo.fullPathHash == Animator.StringToHash("Base.Idle")
            );
            ability.End(instigator);
        }

        private static Dictionary<string, ApplyEffect> logics = new Dictionary<string, ApplyEffect>();
        public static AbilityLogic Get(string abilityId)
        {
            if (!logics.TryGetValue(abilityId, out var logic)) {
                logic = ScriptableObject.CreateInstance(typeof(ApplyEffect)) as ApplyEffect;
                logics.Add(abilityId, logic);

                if (abilityId == ID.ability_bloodPact) {
                    Effect effect = Effect.Get(EffectConfigs.GetNormalConfig(ID.effect_bloodPact));
                    logic.SetData( true, effect);

                } else if (abilityId == ID.ability_heal) {
                    Effect effect = Effect.Get(EffectConfigs.GetNormalConfig(ID.effect_heal));
                    logic.SetData( false, effect);
                }
            }
            return logic;
        }

    }
}
