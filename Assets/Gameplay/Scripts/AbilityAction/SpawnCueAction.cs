
using System;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Effects;
using UnityEngine;
using UnityEngine.Serialization;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

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
            Transform go;
            if (string.IsNullOrEmpty(objectKey)) {
                go = Instantiate(particleToSpawn, target.transform).transform;
            } else {
                go = await Addressables.LoadAssetAsync<Transform>(objectKey);
            }
            // Time.timeScale = 0.5f;
            // await UniTask.DelayFrame(5);
            // Time.timeScale = 1;
            if (destroyInSeconds > 0) {
                await UniTask.Delay(TimeSpan.FromSeconds(destroyInSeconds));
                GameObject.DestroyImmediate(go.gameObject);
            }
        }
    }
}