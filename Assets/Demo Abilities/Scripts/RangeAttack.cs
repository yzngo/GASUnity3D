using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbilitySystem.ExtensionMethods;
using AbilitySystem.GameplayEffects;
using AbilitySystem.Interfaces;
using UniRx.Async;
using UnityEngine;

namespace AbilitySystem.Abilities.AbilityActivations {
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System Demo/Ability Logic/Range Attack")]
    public class RangeAttack : AbstractAbilityActivation {

        public GameObject Projectile;
        public Vector3 ProjectilePositionOffset;

        public GameplayEffect TargetGameplayEffect;
        public AnimationEvent CastingInitiated;
        public AnimationEvent FireProjectile;
        public GameplayTag WaitForEventTag;
        public string AnimationTriggerName;
        public string ProjectileFireTriggerName;
        public string CompletionAnimatorStateFullHash;

        public override async void ActivateAbility(AbilitySystemComponent ASC, IGameplayAbility Ability) {
            var abilitySystemActor = ASC.transform;
            var animationEventSystemComponent = abilitySystemActor.GetComponent<AnimationEventSystem>();
            var animatorComponent = abilitySystemActor.GetComponent<Animator>();

            // Make sure we have enough resources.  End ability if we don't

            (_, var gameplayEventData) = await ASC.OnGameplayEvent.WaitForEvent((gameplayTag, eventData) => gameplayTag == WaitForEventTag);
            animatorComponent.SetTrigger(AnimationTriggerName);

            List<GameObject> objectsSpawned = new List<GameObject>();

            GameObject instantiatedProjectile = null;
            
            await animationEventSystemComponent.CustomAnimationEvent.WaitForEvent((x) => x == CastingInitiated);

            if (Projectile != null) {
                instantiatedProjectile = Instantiate(Projectile);
                instantiatedProjectile.transform.position = abilitySystemActor.transform.position + this.ProjectilePositionOffset + abilitySystemActor.transform.forward * 1.2f;
            }

            animatorComponent.SetTrigger(ProjectileFireTriggerName);


            await animationEventSystemComponent.CustomAnimationEvent.WaitForEvent((x) => x == FireProjectile);



            // Animation complete.  Spawn and send projectile at target
            if (instantiatedProjectile != null) {
                SeekTargetAndDestroy(ASC, gameplayEventData, instantiatedProjectile);
            }


            var beh = animatorComponent.GetBehaviour<AnimationBehaviourEventSystem>();
            await beh.StateEnter.WaitForEvent((animator, stateInfo, layerIndex) => stateInfo.fullPathHash == Animator.StringToHash(CompletionAnimatorStateFullHash));

            Ability.EndAbility(ASC);
        }

        private async void SeekTargetAndDestroy(AbilitySystemComponent ASC, GameplayEventData gameplayEventData, GameObject projectile) {
            await projectile.GetComponent<Projectile>().SeekTarget(gameplayEventData.Target.TargetPoint, gameplayEventData.Target.gameObject);
            _ = ASC.ApplyEffectToTarget(TargetGameplayEffect, gameplayEventData.Target);
            DestroyImmediate(projectile);
        }

    }
}
