using GameplayAbilitySystem.Effects;
using UnityEngine;
using GameplayAbilitySystem.Utility;

namespace GameplayAbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System Demo/Ability Logic/Instant Attack")]
    public class InstantAttack : AbilityLogic 
    {
        public Effect TargetGameplayEffect;
        public string ExecuteEffectToken;
        public GameplayTag WaitForEventTag;
        public string AnimationTriggerName;
        public string AnimationCompleteTriggerName;
        public string CompletionAnimatorStateFullHash;

        public override async void Execute(AbilitySystem instigator, Ability Ability) 
        {
            var animator = instigator.Animator;

            // Make sure we have enough resources.  End ability if we don't

            var abilityEventData = await instigator.OnAbilityEvent.WaitForEvent((eventData) => eventData.abilityTag == WaitForEventTag);
            animator.SetTrigger(AnimationTriggerName);
            animator.SetTrigger(AnimationCompleteTriggerName);

            if ( !string.IsNullOrEmpty(ExecuteEffectToken) ) {
                await instigator.OnAnimEvent.WaitForEvent( (x) => x == ExecuteEffectToken);
            }
            instigator.ApplyEffectToTarget(TargetGameplayEffect, abilityEventData.target);


            var beh = animator.GetBehaviour<ActorFSMBehaviour>();
            await beh.StateEnter.WaitForEvent((anim, stateInfo, layerIndex) => stateInfo.fullPathHash == Animator.StringToHash(CompletionAnimatorStateFullHash));

            Ability.End(instigator);
        }

    }
}
