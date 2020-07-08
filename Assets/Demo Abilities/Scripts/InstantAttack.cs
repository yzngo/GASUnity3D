using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GAS.ExtensionMethods;
using GAS.GameplayEffects;
using GAS.Interfaces;
using UniRx.Async;
using UnityEngine;

namespace GAS.Abilities.AbilityActivations {
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System Demo/Ability Logic/Instant Attack")]
    public class InstantAttack : AbstractAbilityActivation {

        public GameplayEffect TargetGameplayEffect;
        public AnimationEvent ExecuteEffectEvent;
        public GameplayTag WaitForEventTag;
        public string AnimationTriggerName;
        public string AnimationCompleteTriggerName;
        public string CompletionAnimatorStateFullHash;

        public override async void ActivateAbility(AbilitySystemComponent AbilitySystem, IGameplayAbility Ability) {
            var abilitySystemActor = AbilitySystem.GetActor();
            var animationEventSystemComponent = abilitySystemActor.GetComponent<AnimationEventSystem>();
            var animatorComponent = abilitySystemActor.GetComponent<Animator>();

            // Make sure we have enough resources.  End ability if we don't

            (_, var gameplayEventData) = await AbilitySystem.OnGameplayEvent.WaitForEvent((gameplayTag, eventData) => gameplayTag == WaitForEventTag);
            animatorComponent.SetTrigger(AnimationTriggerName);
            animatorComponent.SetTrigger(AnimationCompleteTriggerName);

            if (ExecuteEffectEvent != null) {
                await animationEventSystemComponent.CustomAnimationEvent.WaitForEvent((x) => x == ExecuteEffectEvent);
            }
            _ = AbilitySystem.ApplyGameEffectToTarget(TargetGameplayEffect, gameplayEventData.Target);


            var beh = animatorComponent.GetBehaviour<AnimationBehaviourEventSystem>();
            await beh.StateEnter.WaitForEvent((animator, stateInfo, layerIndex) => stateInfo.fullPathHash == Animator.StringToHash(CompletionAnimatorStateFullHash));

            // End Ability
            Ability.EndAbility(AbilitySystem);
        }

    }
}
