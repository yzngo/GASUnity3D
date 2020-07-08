using System.Threading.Tasks;
using GAS.ExtensionMethods;
using GAS.Interfaces;
using UnityEngine;

namespace GAS.Abilities.AbilityActivations {
    public abstract class AbstractAbilityActivation : ScriptableObject {
        public abstract void ActivateAbility(AbilitySystemComponent AbilitySystem, IGameplayAbility Ability);
    }
}