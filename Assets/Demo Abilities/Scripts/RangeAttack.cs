using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.Effects;
using UniRx.Async;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Logic 
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System Demo/Ability Logic/Range Attack")]
    public class RangeAttack : AbilityLogic 
    {
        public GameObject Projectile;
        public Vector3 ProjectilePositionOffset;
        public Effect TargetGameplayEffect;
        public AnimationEvent CastingInitiated;
        public AnimationEvent FireProjectile;
        public GameplayTag WaitForEventTag;
        public string AnimationTriggerName;
        public string ProjectileFireTriggerName;
        public string CompletionAnimatorStateFullHash;

        public override async void Execute(AbilitySystem abilitySystem, Ability Ability) {
            var abilitySystemActor = abilitySystem.transform;
            var animationEventSystemComponent = abilitySystemActor.GetComponent<AnimationEventSystem>();
            var animatorComponent = abilitySystemActor.GetComponent<Animator>();

            // Make sure we have enough resources.  End ability if we don't
            var abilityEventData = await abilitySystem.OnAbilityEvent.WaitForEvent((eventData) => eventData.AbilityTag == WaitForEventTag);
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
                SeekTargetAndDestroy(abilitySystem, abilityEventData.Target, instantiatedProjectile);
            }


            var beh = animatorComponent.GetBehaviour<AnimationBehaviourEventSystem>();
            await beh.StateEnter.WaitForEvent((animator, stateInfo, layerIndex) => stateInfo.fullPathHash == Animator.StringToHash(CompletionAnimatorStateFullHash));

            Ability.EndAbility(abilitySystem);
        }

        private async void SeekTargetAndDestroy(AbilitySystem abilitySystem, AbilitySystem target, GameObject projectile) {
            await projectile.GetComponent<Projectile>().SeekTarget(target.TargetPoint, target.gameObject);
            abilitySystem.ApplyEffectToTarget(TargetGameplayEffect, target);
            DestroyImmediate(projectile);
        }

    }
}
