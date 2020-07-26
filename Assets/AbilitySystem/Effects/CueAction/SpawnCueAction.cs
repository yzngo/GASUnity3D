
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
            if (destroyInSeconds > 0) {
                await UniTask.Delay(TimeSpan.FromSeconds(destroyInSeconds));
                GameObject.DestroyImmediate(go.gameObject);
            }
        }

        private static Dictionary<string, SpawnCueAction> actions = new Dictionary<string, SpawnCueAction>();
        public static BaseCueAction Get(string id)
        {
            if (!actions.TryGetValue(id, out var action)) {
                action = ScriptableObject.CreateInstance(typeof(SpawnCueAction)) as SpawnCueAction;
                actions.Add(id, action);
                TestData.spawnCueData.TryGetValue(id, out var data);
                action.objectKey = data.objectToSpawn;
                action.ResetDestroyTime(data.destroyTime);
            }
            return action;
        }
    }
    
}