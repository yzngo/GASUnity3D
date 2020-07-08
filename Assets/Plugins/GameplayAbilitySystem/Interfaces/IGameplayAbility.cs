using System.Collections.Generic;
using System.Threading.Tasks;
using GAS;
using GAS.Abilities.AbilityActivations;
using GAS.Abilities;
using GAS.GameplayEffects;
using UnityEngine;
using UnityEngine.Events;

namespace GAS.Interfaces {
    /// <summary>
    /// This interfaces defines Gameplay Abilities.  Gameplay Abilities represent "things" that players can cast, etc.
    /// E.g. a <see cref="IGameplayAbility"/> might represent a fireball ability which the player casts and which damages a target
    /// </summary>
    public interface IGameplayAbility {
        /// <summary>
        /// Tags that this ability has/provides
        /// </summary>
        /// <value></value>
        IAbilityTags Tags { get; }

        /// <summary>
        /// Cost of using this ability
        /// </summary>
        /// <value></value>
        IGameplayCost Cost { get; }

        /// <summary>
        /// Cooldowns associated with this ability
        /// </summary>
        /// <value></value>
        List<GameplayEffect> Cooldowns { get; }

        /// <summary>
        /// This is called whenever this ability ends.
        /// This event does not pass on details of which <see cref="AbilitySystemComponent"/> 
        /// was responsible for using this ability.
        /// </summary>
        /// <value></value>
        // GenericAbilityEvent OnGameplayAbilityEnded { get; }

        /// <summary>
        /// This is called whenever this ability is commited.
        /// This event does not pass on details of which <see cref="AbilitySystemComponent"/> 
        /// was responsible for using this ability.
        /// </summary>
        /// <value></value>
        // GenericAbilityEvent OnGameplayAbilityCommitted { get; }

        /// <summary>
        /// This is called whenever this ability is cancelled.
        /// This event does not pass on details of which <see cref="AbilitySystemComponent"/> 
        /// was responsible for using this ability.
        /// </summary>
        /// <value></value>
        // GenericAbilityEvent OnGameplayAbilityCancelled { get; }

        /// <summary>
        /// Defines what the ability actually does
        /// </summary>
        /// <value></value>        
        AbstractAbilityActivation AbilityLogic { get; }

        /// <summary>
        /// Ends this ability on the target <see cref="AbilitySystemComponent"/>
        /// </summary>
        /// <param name="AbilitySystem">The target <see cref="AbilitySystemComponent"/></param>
        void EndAbility(AbilitySystemComponent AbilitySystem);

        /// <summary>
        /// Activates this ability on the target <see cref="AbilitySystemComponent"/>
        /// </summary>
        /// <param name="AbilitySystem">The target <see cref="AbilitySystemComponent"/></param>
        void ActivateAbility(AbilitySystemComponent AbilitySystem);

        /// <summary>
        /// Check if this ability can be activated by <see cref="AbilitySystemComponent"/>
        /// </summary>
        /// <param name="AbilitySystem">The target <see cref="AbilitySystemComponent"/></param>
        /// <returns></returns>
        bool IsAbilityActivatable(AbilitySystemComponent AbilitySystem);

        /// <summary>
        /// Commits the <see cref="AbilitySystemComponent"/> on the target <see cref="AbilitySystemComponent"/>
        /// </summary>
        /// <param name="AbilitySystem">The target <see cref="AbilitySystemComponent"/></param>
        /// <returns></returns>
        bool CommitAbility(AbilitySystemComponent AbilitySystem);

        (float CooldownElapsed, float CooldownTotal) CalculateCooldown(AbilitySystemComponent AbilitySystem);
    }
}
