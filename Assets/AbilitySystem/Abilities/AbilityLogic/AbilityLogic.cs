using System.Threading.Tasks;
using UnityEngine;

namespace GameplayAbilitySystem
{
    public abstract class AbilityLogic : ScriptableObject 
    {
        public abstract void Execute(AbilitySystem instigator, Ability ability);
    }
}