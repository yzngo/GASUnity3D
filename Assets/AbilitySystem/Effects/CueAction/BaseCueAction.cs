using UnityEngine;
namespace GameplayAbilitySystem
{
    public abstract class BaseCueAction : ScriptableObject 
    {
        public abstract void Execute(AbilitySystem target); 
    }
}
