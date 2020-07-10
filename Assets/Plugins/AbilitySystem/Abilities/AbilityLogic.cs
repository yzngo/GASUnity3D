using System.Threading.Tasks;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.AbilityActivations 
{
    public abstract class AbilityLogic : ScriptableObject 
    {
        public abstract void Execute(AbilitySystem instigator, Ability ability);
    }
}