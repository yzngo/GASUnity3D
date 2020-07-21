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
        public string CastingInitiatedToken;
        public string FireProjectileToken;
        public string AnimationTriggerName;
        public string ProjectileFireTriggerName;
        public string CompletionAnimatorStateFullHash;

        public override async void Execute(AbilitySystem instigator, Ability ability) 
        {
            // var abilitySystemActor = instigator.transform;
            var animatorComponent = instigator.GetComponent<Animator>();

            animatorComponent.SetTrigger(AnimationTriggerName);

            List<GameObject> objectsSpawned = new List<GameObject>();

            GameObject instantiatedProjectile = null;
            await instigator.OnAnimEvent.WaitForEvent( (x) => x == CastingInitiatedToken );

            if (Projectile != null) {
                instantiatedProjectile = Instantiate(Projectile);
                instantiatedProjectile.transform.position = instigator.transform.position + this.ProjectilePositionOffset + instigator.transform.forward * 1.2f;
            }

            animatorComponent.SetTrigger(ProjectileFireTriggerName);

            await instigator.OnAnimEvent.WaitForEvent( (x) => x == FireProjectileToken );

            // Animation complete.  Spawn and send projectile at target
            if (instantiatedProjectile != null) {
                SeekTargetAndDestroy(instigator, ability, instantiatedProjectile);
            }

            var beh = animatorComponent.GetBehaviour<ActorFSMBehaviour>();
            await beh.StateEnter.WaitForEvent((animator, stateInfo, layerIndex) => stateInfo.fullPathHash == Animator.StringToHash(CompletionAnimatorStateFullHash));
            ability.End(instigator);
        }

        private async void SeekTargetAndDestroy(AbilitySystem instigator, Ability ability, GameObject projectile) {
            await projectile.GetComponent<Projectile>().SeekTarget(ability.Target.TargetPoint, ability.Target.gameObject);
            instigator.ApplyEffectToTarget(ability.Id, TargetGameplayEffect, ability.Target);
            DestroyImmediate(projectile);
        }

    }
}
