using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.Effects;
using GameplayAbilitySystem.Interfaces;
using UniRx.Async;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.AbilityActivations {
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System Demo/Ability Logic/Instant Attack")]
    public class InstantAttack : AbstractAbilityActivation {

        public Effect TargetGameplayEffect;
        public AnimationEvent ExecuteEffectEvent;
        public GameplayTag WaitForEventTag;
        public string AnimationTriggerName;
        public string AnimationCompleteTriggerName;
        public string CompletionAnimatorStateFullHash;

        public override async void ActivateAbility(AbilitySystem instigator, Ability Ability) {
            var animationEventSystem = instigator.GetComponent<AnimationEventSystem>();
            var animator = instigator.Animator;

            // Make sure we have enough resources.  End ability if we don't

            (_, var abilityEventData) = await instigator.OnAbilityEvent.WaitForEvent((gameplayTag, eventData) => gameplayTag == WaitForEventTag);
            animator.SetTrigger(AnimationTriggerName);
            animator.SetTrigger(AnimationCompleteTriggerName);

            if (ExecuteEffectEvent != null) {
                await animationEventSystem.CustomAnimationEvent.WaitForEvent((x) => x == ExecuteEffectEvent);
            }
            instigator.ApplyEffectToTarget(TargetGameplayEffect, abilityEventData.Target);


            var beh = animator.GetBehaviour<AnimationBehaviourEventSystem>();
            await beh.StateEnter.WaitForEvent((anim, stateInfo, layerIndex) => stateInfo.fullPathHash == Animator.StringToHash(CompletionAnimatorStateFullHash));

            Ability.EndAbility(instigator);
        }

    }
}
