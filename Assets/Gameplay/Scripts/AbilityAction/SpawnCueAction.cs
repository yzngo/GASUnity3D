
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
        public GameObject ObjectToSpawn = default;
        public float DestroyInSeconds = -1;

        public override async void Execute(AbilitySystem target) 
        {
            Time.timeScale = 0.5f;
            await UniTask.DelayFrame(5);
            Time.timeScale = 1;

            Transform go = Instantiate(ObjectToSpawn, target.transform).transform;
            if (DestroyInSeconds > 0) {
                await UniTask.Delay(TimeSpan.FromSeconds(DestroyInSeconds));
                GameObject.DestroyImmediate(go.gameObject);
            }
        }

    }
}