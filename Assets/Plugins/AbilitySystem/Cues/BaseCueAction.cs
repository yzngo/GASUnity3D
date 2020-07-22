using UnityEngine;
using GameplayAbilitySystem;
namespace GameplayAbilitySystem.Cues {

    public abstract class BaseCueAction : ScriptableObject 
    {
        public abstract void Execute(AbilitySystem target); 
    }
}
