using GameplayAbilitySystem.Effects;
using UnityEngine;
using GameplayAbilitySystem.Utility;

namespace GameplayAbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System Demo/Ability Logic/Instant Attack")]
    public class InstantAttack : AbilityLogic 
    {
        [SerializeField] private bool waitForCastingStart = false;
        [SerializeField] private bool waitForFireProjectile = false;
        [SerializeField] private bool waitForCastingComplete = false;
        [SerializeField] private Effect appliedEffectAfterComplete = default;

        public override async void Execute(AbilitySystem instigator, Ability ability) 
        {
            var animator = instigator.Animator;

            AbilityEventData abilityEventData = await instigator.OnAbilityStart.WaitForEvent(
                (eventData) => eventData.abilityId == ability.Id
            );

            animator.SetTrigger(AnimParams.Do_Magic);
            animator.SetTrigger(AnimParams.Execute_Magic_2);

            if ( waitForCastingComplete == true) {
                await instigator.OnAnimEvent.WaitForEvent( (x) => x == AnimEventKey.CastingComplete);
            }

            instigator.ApplyEffectToTarget(ability.Id, appliedEffectAfterComplete, abilityEventData.target);

            ActorFSMBehaviour fsmBehaviour = animator.GetBehaviour<ActorFSMBehaviour>();
            await fsmBehaviour.StateEnter.WaitForEvent(
                (anim, stateInfo, layerIndex) => 
                    stateInfo.fullPathHash == Animator.StringToHash("Base.Idle")
            );
            ability.End(instigator);
        }

    }
}
