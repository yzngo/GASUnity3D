
using System;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Cues;
using UnityEngine;
using UnityEngine.Serialization;
using Cysharp.Threading.Tasks;


namespace AbilitySystemDemo 
{
    [CreateAssetMenu(fileName = "Spawn Object Gameplay Cue", menuName = "Ability System Demo/Gameplay Cue/Spawn Object Gameplay Cue")]
    class SpawnCueAction : BaseCueAction 
    {
        [SerializeField] private GameObject particleToSpawn = default;
        [SerializeField] private float destroyInSeconds = -1;

        public override async void Execute(AbilitySystem target) 
        {
            // Time.timeScale = 0.5f;
            // await UniTask.DelayFrame(5);
            // Time.timeScale = 1;
            Transform go = Instantiate(particleToSpawn, target.transform).transform;
            if (destroyInSeconds > 0) {
                await UniTask.Delay(TimeSpan.FromSeconds(destroyInSeconds));
                GameObject.DestroyImmediate(go.gameObject);
            }
        }
    }
}