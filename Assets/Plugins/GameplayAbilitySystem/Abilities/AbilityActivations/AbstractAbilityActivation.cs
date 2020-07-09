using System.Threading.Tasks;
using AbilitySystem.ExtensionMethods;
using AbilitySystem.Interfaces;
using UnityEngine;

namespace AbilitySystem.Abilities.AbilityActivations {
    public abstract class AbstractAbilityActivation : ScriptableObject {
        public abstract void ActivateAbility(AbilitySystemComponent ASC, IGameplayAbility Ability);
    }
}