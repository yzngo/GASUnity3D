using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameplayAbilitySystem
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Ability System/Ability Logic/Tracking Attack")]
    public class TrackingAttack : AbilityLogic 
    {
        [SerializeField] private GameObject projectile = default;


        [Tooltip("相对于发出者的偏移位置")]
        [SerializeField] private Vector3 projectilePositionOffset = default;
        [SerializeField] private Effect appliedEffectAfterComplete = default;

        private string projectileKey = "";

        public void SetData(string projectileKey, Vector3 offset, Effect effect)
        {
            this.projectileKey = projectileKey;
            projectilePositionOffset = offset;
            appliedEffectAfterComplete = effect;
        }

        public override async void Execute(AbilitySystem instigator, Ability ability) 
        {
            var animator = instigator.GetComponent<Animator>();

            AbilityEventData abilityEventData = await instigator.OnAbilityStart.WaitForEvent(
                (eventData) => eventData.abilityId == ability.Id
            );
            animator.SetTrigger(AnimParams.Do_Magic);

            GameObject instantiatedProjectile = null;
            await instigator.OnAnimEvent.WaitForEvent( (x) => x == AnimEventKey.CastingStart );

            if (projectile == null) {
                projectile = Resources.Load<GameObject>(projectileKey);
            }
            instantiatedProjectile = Instantiate(projectile);
            instantiatedProjectile.transform.position = instigator.transform.position + projectilePositionOffset + instigator.transform.forward * 1.2f;

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
            await projectile.GetComponent<Projectile>().SeekTarget(data.target.transform, data.target.gameObject);
            instigator.ApplyEffectToTarget(data.ability.Id, appliedEffectAfterComplete, data.target);
            DestroyImmediate(projectile);
        }

        private static Dictionary<string, TrackingAttack> logics = new Dictionary<string, TrackingAttack>();
        public static AbilityLogic Get(string abilityId)
        {
            if (!logics.TryGetValue(abilityId, out var logic)) {
                logic = ScriptableObject.CreateInstance("TrackingAttack") as TrackingAttack;
                logics.Add(abilityId, logic);
                
                if (abilityId == ID.ability_fire) {
                    Effect effect = Effect.Get(EffectConfigs.GetNormalConfig(ID.effect_fire));
                    logic.SetData(AddressKey.Fireball, new Vector3(0, 1.5f, 0), effect);
                }
            }
            return logic;
        }
    }
}
