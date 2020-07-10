using System.Threading.Tasks;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Logic 
{
    public abstract class AbilityLogic : ScriptableObject 
    {
        public abstract void Execute(AbilitySystem instigator, Ability ability);
    }
}