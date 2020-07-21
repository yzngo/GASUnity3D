
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

        [FormerlySerializedAs("DestroyInSeconds")]
        [SerializeField] private float destroyInSeconds = -1;

        public override async void Execute(AbilitySystem target) 
        {
            // Time.timeScale = 0.5f;
            // await UniTask.DelayFrame(5);
            // Time.timeScale = 1;
            Transform go = Instantiate(particleToSpawn, target.transform).transform;
            // ParticleSystem[] particleSystems = target.GetComponentsInChildren<ParticleSystem>();
            //     bool allStopped = false;
            //     while(!allStopped) {
            //         if (!particleSystems[0].isStopped) {
            //             allStopped = false;
            //             await UniTask.DelayFrame(0);
            //         } else {
            //             GameObject.DestroyImmediate(go.gameObject);
            //             break;
            //         }
            //     }
            // float length = ParticleSystemLength(target.transform);
            if (destroyInSeconds > 0) {
                await UniTask.Delay(TimeSpan.FromSeconds(destroyInSeconds));
                GameObject.DestroyImmediate(go.gameObject);
            }
        }
        // private float ParticleSystemLength(Transform transform)
        // {
        //     ParticleSystem[] particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
        //     float maxDuration = 0f;

        //     foreach(var p in particleSystems) {
        //         if(p.emission.enabled) {
        //             if(p.main.loop) {
        //                 return -1f;
        //             }

        //             float duration = 0f;
        //             if(p.emission.rateOverTime.constantMax <= 0) {
        //                 duration = p.main.startDelay.constantMax + p.main.startLifetime.constantMax;
        //             } else {
        //                 duration = p.main.startDelay.constantMax + Mathf.Max(p.main.duration, p.main.startLifetime.constantMax);
        //                 // Debug.Log(p.main.startDelay.constantMax + " " + p.main.duration + " " + p.main.startLifetime.constantMax);
        //             }

        //             if (duration > maxDuration) {
        //                 maxDuration = duration;
        //             }
        //         }
        //     }
        //     Debug.Log("max duration: " + maxDuration);
        //     return maxDuration;
        // }

    }
}