using UnityEngine;
using GameplayAbilitySystem;
namespace GameplayAbilitySystem.Effects 
{
    public abstract class BaseCueAction : ScriptableObject 
    {
        public abstract void Execute(AbilitySystem target); 
    }
}
