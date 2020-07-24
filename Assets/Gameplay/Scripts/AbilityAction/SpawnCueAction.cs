
using System;
using UnityEngine;
using UnityEngine.Serialization;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

namespace GameplayAbilitySystem 
{
    [CreateAssetMenu(fileName = "Spawn Object Cue", menuName = "Ability System/Cues/Action/Spawn Object Gameplay Cue")]
    class SpawnCueAction : BaseCueAction 
    {
        [SerializeField] private GameObject particleToSpawn = default;
        [SerializeField] private float destroyInSeconds = -1;

        public string objectKey;

        public void ResetDestroyTime(float time)
        {
            destroyInSeconds = time;
        }
        
        public override async void Execute(AbilitySystem target) 
        {
            if (particleToSpawn == null) {
                particleToSpawn = Resources.Load<GameObject>("Particle/" + objectKey);
            }
            Transform go;
            go = Instantiate(particleToSpawn, target.transform).transform;
            // Time.timeScale = 0.5f;
            // await UniTask.DelayFrame(5);
            // Time.timeScale = 1;
            if (destroyInSeconds > 0) {
                await UniTask.Delay(TimeSpan.FromSeconds(destroyInSeconds));
                GameObject.DestroyImmediate(go.gameObject);
            }
        }

        private static Dictionary<string, SpawnCueAction> actions = new Dictionary<string, SpawnCueAction>();
        public static BaseCueAction Get(string id)
        {
            if (!actions.TryGetValue(id, out var action)) {
                action = ScriptableObject.CreateInstance("SpawnCueAction") as SpawnCueAction;
                actions.Add(id, action);
                if (id == ID.cue_manaSurgeZ) {
                    action.objectKey =  AddressKey.EnergyExplosionRay;
                    action.ResetDestroyTime(2);

                } else if (id == ID.cue_regenHealthSprite) {
                    action.objectKey = AddressKey.Sprites;
                    action.ResetDestroyTime(3);

                } else if (id == ID.cue_regenHealth) {
                    action.objectKey = AddressKey.MagicCircle;
                    action.ResetDestroyTime(1);

                } else if (id == ID.cue_spawnBigExplosion) {
                    action.objectKey = AddressKey.FireExplosion;
                    action.ResetDestroyTime(2);

                } else if (id == ID.cue_spawnEnergyExplosion) {
                    action.objectKey = AddressKey.EnergyExplosion;
                    action.ResetDestroyTime(2);

                } else if (id == ID.cue_spawnMagicCircle) {
                    action.objectKey = AddressKey.MagicCircle;
                    action.ResetDestroyTime(3);
                }
            }
            return action;
        }
    }
    
}