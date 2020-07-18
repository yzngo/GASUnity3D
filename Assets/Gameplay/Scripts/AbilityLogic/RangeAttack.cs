using System.Collections.Generic;
using GameplayAbilitySystem.Effects;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GameplayAbilitySystem.Utility;

namespace GameplayAbilitySystem.Abilities
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

        public override async void Execute(AbilitySystem instigator, Ability ability) {
            var abilitySystemActor = instigator.transform;
            var animationEventSystemComponent = abilitySystemActor.GetComponent<AnimationEventSystem>();
            var animatorComponent = abilitySystemActor.GetComponent<Animator>();

            // Make sure we have enough resources.  End ability if we don't
            var abilityEventData = await instigator.OnAbilityEvent.WaitForEvent((eventData) => eventData.abilityTag == WaitForEventTag);
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
                SeekTargetAndDestroy(instigator, abilityEventData.target, instantiatedProjectile);
            }


            var beh = animatorComponent.GetBehaviour<ActorFSMBehaviour>();
            await beh.StateEnter.WaitForEvent((animator, stateInfo, layerIndex) => stateInfo.fullPathHash == Animator.StringToHash(CompletionAnimatorStateFullHash));

            ability.End(instigator);
        }

        private async void SeekTargetAndDestroy(AbilitySystem abilitySystem, AbilitySystem target, GameObject projectile) {
            await projectile.GetComponent<Projectile>().SeekTarget(target.TargetPoint, target.gameObject);
            abilitySystem.ApplyEffectToTarget(TargetGameplayEffect, target);
            DestroyImmediate(projectile);
        }

    }
}
