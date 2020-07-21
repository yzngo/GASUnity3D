using System.Collections.Generic;
using GameplayAbilitySystem.Effects;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GameplayAbilitySystem.Utility;

namespace GameplayAbilitySystem.Abilities
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System Demo/Ability Logic/Tracking Attack")]
    public class TrackingAttack : AbilityLogic 
    {
        [SerializeField] private GameObject projectile = default;

        [Tooltip("相对于发出者的偏移位置")]
        [SerializeField] private Vector3 projectilePositionOffset = default;
        [SerializeField] private Effect appliedEffectAfterComplete = default;

        public override async void Execute(AbilitySystem instigator, Ability ability) 
        {
            var animator = instigator.GetComponent<Animator>();

            AbilityEventData abilityEventData = await instigator.OnAbilityStart.WaitForEvent(
                (eventData) => eventData.abilityId == ability.Id
            );
            animator.SetTrigger(AnimParams.Do_Magic);

            GameObject instantiatedProjectile = null;
            await instigator.OnAnimEvent.WaitForEvent( (x) => x == AnimEventKey.CastingStart );

            if (projectile != null) {
                instantiatedProjectile = Instantiate(projectile);
                instantiatedProjectile.transform.position = instigator.transform.position + projectilePositionOffset;
            }

            animator.SetTrigger(AnimParams.Execute_Magic);
            await instigator.OnAnimEvent.WaitForEvent( (x) => x == AnimEventKey.FireProjectile );

            if (instantiatedProjectile != null) {
                SeekTargetAndDestroy(instigator, abilityEventData, instantiatedProjectile);
            }

            ActorFSMBehaviour fsmBehaviour = animator.GetBehaviour<ActorFSMBehaviour>();
            await fsmBehaviour.StateEnter.WaitForEvent((_, stateInfo, _2) => stateInfo.fullPathHash == Animator.StringToHash("Base.Idle"));
            ability.End(instigator);
        }

        private async void SeekTargetAndDestroy(AbilitySystem instigator, AbilityEventData data, GameObject projectile) {
            await projectile.GetComponent<Projectile>().SeekTarget(data.target.TargetPoint, data.target.gameObject);
            instigator.ApplyEffectToTarget(data.ability.Id, appliedEffectAfterComplete, data.target);
            DestroyImmediate(projectile);
        }
    }
}
