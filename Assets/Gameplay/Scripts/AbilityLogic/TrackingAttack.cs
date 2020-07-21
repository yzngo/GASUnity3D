using System.Collections.Generic;
using GameplayAbilitySystem.Effects;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GameplayAbilitySystem.Utility;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System Demo/Ability Logic/Tracking Attack")]
    public class TrackingAttack : AbilityLogic 
    {
        [FormerlySerializedAs("Projectile")]
        public GameObject projectile;

        [FormerlySerializedAs("ProjectilePositionOffset")]
        public Vector3 projectilePositionOffset;

        [FormerlySerializedAs("TargetGameplayEffect")]
        public Effect appliedEffectAfterComplete;


        public override async void Execute(AbilitySystem instigator, Ability ability) 
        {
            var animator = instigator.GetComponent<Animator>();

            AbilityEventData abilityEventData = await instigator.OnAbilityStart.WaitForEvent(
                (eventData) => eventData.abilityId == ability.Id
            );

            List<GameObject> objectsSpawned = new List<GameObject>();

            animator.SetTrigger(AnimParams.Do_Magic);

            GameObject instantiatedProjectile = null;
            await instigator.OnAnimEvent.WaitForEvent( (x) => x == AnimEventKey.CastingStart );

            if (projectile != null) {
                instantiatedProjectile = Instantiate(projectile);
                instantiatedProjectile.transform.position = 
                        instigator.transform.position + projectilePositionOffset + instigator.transform.forward * 1.2f;
            }
            animator.SetTrigger(AnimParams.Execute_Magic);

            await instigator.OnAnimEvent.WaitForEvent( (x) => x == AnimEventKey.FireProjectile );

            // Animation complete.  Spawn and send projectile at target
            if (instantiatedProjectile != null) {
                SeekTargetAndDestroy(instigator, abilityEventData, instantiatedProjectile);
            }

            var beh = animator.GetBehaviour<ActorFSMBehaviour>();
            await beh.StateEnter.WaitForEvent((_1, stateInfo, _2) => stateInfo.fullPathHash == Animator.StringToHash("Base.Idle"));
            ability.End(instigator);
        }

        private async void SeekTargetAndDestroy(AbilitySystem instigator, AbilityEventData data, GameObject projectile) {
            await projectile.GetComponent<Projectile>().SeekTarget(data.target.TargetPoint, data.target.gameObject);
            instigator.ApplyEffectToTarget(data.ability.Id, appliedEffectAfterComplete, data.target);
            DestroyImmediate(projectile);
        }

    }
}
