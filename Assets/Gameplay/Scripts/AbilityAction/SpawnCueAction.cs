
using System;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Effects;
using UnityEngine;
using UnityEngine.Serialization;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

namespace AbilitySystemDemo 
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
                if (id == ID.manaSurgeZ) {
                    action.objectKey =  AddressKey.EnergyExplosionRay;
                    action.ResetDestroyTime(2);

                } else if (id == ID.regenHealthSprite) {
                    action.objectKey = AddressKey.Sprites;
                    action.ResetDestroyTime(3);

                } else if (id == ID.regenHealth) {
                    action.objectKey = AddressKey.MagicCircle;
                    action.ResetDestroyTime(1);

                } else if (id == ID.spawnBigExplosion) {
                    action.objectKey = AddressKey.FireExplosion;
                    action.ResetDestroyTime(2);

                } else if (id == ID.spawnEnergyExplosion) {
                    action.objectKey = AddressKey.EnergyExplosion;
                    action.ResetDestroyTime(2);

                } else if (id == ID.spawnMagicCircle) {
                    action.objectKey = AddressKey.MagicCircle;
                    action.ResetDestroyTime(3);
                }
            }
            return action;
        }
    }
        // id 之后全部改成读表
        public static partial class ID {
            public static string manaSurgeZ= "manaSurgeZ";
            public static string regenHealthSprite= "regenHealthSprite";
            public static string regenHealth= "regenHealth";
            public static string spawnBigExplosion= "spawnBigExplosion";
            public static string spawnEnergyExplosion= "spawnEnergyExplosion";
            public static string spawnMagicCircle= "spawnMagicCircle";
        }
        
        public static partial class AddressKey {
            public static string EnergyExplosionRay = "EnergyExplosionRay";
            public static string Sprites = "Sprites";
            public static string MagicCircle= "MagicCircle";
            public static string FireExplosion= "FireExplosion";
            public static string EnergyExplosion= "EnergyExplosion";
        }
}