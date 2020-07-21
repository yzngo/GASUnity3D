using GameplayAbilitySystem.Effects;
using UnityEngine;
using GameplayAbilitySystem.Utility;

namespace GameplayAbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System Demo/Ability Logic/Instant Attack")]
    public class InstantAttack : AbilityLogic 
    {
        public Effect TargetGameplayEffect;
        [SerializeField] private bool waitForCastingStart = false;
        [SerializeField] private bool waitForFireProjectile = false;
        [SerializeField] private bool waitForCastingComplete = false;
        [SerializeField] private Effect appliedEffectAfterComplete = default;
        public string AnimationTriggerName;
        public string AnimationCompleteTriggerName;
        public string CompletionAnimatorStateFullHash;

        public override async void Execute(AbilitySystem instigator, Ability ability) 
        {
            var animator = instigator.Animator;
            // Make sure we have enough resources.  End ability if we don't
            AbilityEventData abilityEventData = await instigator.OnAbilityStart.WaitForEvent(
                (eventData) => eventData.abilityId == ability.Id
            );

            animator.SetTrigger(AnimationTriggerName);
            animator.SetTrigger(AnimationCompleteTriggerName);

            if ( waitForCastingComplete == true) {
                await instigator.OnAnimEvent.WaitForEvent( (x) => x == AnimEventKey.CastingComplete);
            }
            instigator.ApplyEffectToTarget(ability.Id, appliedEffectAfterComplete, abilityEventData.target);


            var beh = animator.GetBehaviour<ActorFSMBehaviour>();
            await beh.StateEnter.WaitForEvent((anim, stateInfo, layerIndex) => stateInfo.fullPathHash == Animator.StringToHash(CompletionAnimatorStateFullHash));

            ability.End(instigator);
        }

    }
}
