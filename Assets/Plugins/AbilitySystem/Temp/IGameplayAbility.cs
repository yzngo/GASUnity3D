using System.Collections.Generic;
using System.Threading.Tasks;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Abilities;
using GameplayAbilitySystem.Effects;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilitySystem.Interfaces {
    /// <summary>
    /// This interfaces defines Gameplay Abilities.  Gameplay Abilities represent "things" that players can cast, etc.
    /// E.g. a <see cref="IGameplayAbility"/> might represent a fireball ability which the player casts and which damages a target
    /// </summary>
    // public interface IGameplayAbility {
    //     /// <summary>
    //     /// Tags that this ability has/provides
    //     /// </summary>
    //     /// <value></value>
    //     GameplayAbilityTags Tags { get; }

    //     /// <summary>
    //     /// Cost of using this ability
    //     /// </summary>
    //     /// <value></value>
    //     Effect Cost { get; }

    //     /// <summary>
    //     /// Cooldowns associated with this ability
    //     /// </summary>
    //     /// <value></value>
    //     List<Effect> Cooldowns { get; }

    //     /// <summary>
    //     /// This is called whenever this ability ends.
    //     /// This event does not pass on details of which <see cref="AbilitySystem"/> 
    //     /// was responsible for using this ability.
    //     /// </summary>
    //     /// <value></value>
    //     // GenericAbilityEvent OnGameplayAbilityEnded { get; }

    //     /// <summary>
    //     /// This is called whenever this ability is commited.
    //     /// This event does not pass on details of which <see cref="AbilitySystem"/> 
    //     /// was responsible for using this ability.
    //     /// </summary>
    //     /// <value></value>
    //     // GenericAbilityEvent OnGameplayAbilityCommitted { get; }

    //     /// <summary>
    //     /// This is called whenever this ability is cancelled.
    //     /// This event does not pass on details of which <see cref="AbilitySystem"/> 
    //     /// was responsible for using this ability.
    //     /// </summary>
    //     /// <value></value>
    //     // GenericAbilityEvent OnGameplayAbilityCancelled { get; }

    //     /// <summary>
    //     /// Defines what the ability actually does
    //     /// </summary>
    //     /// <value></value>        
    //     AbstractAbilityActivation AbilityLogic { get; }

    //     /// <summary>
    //     /// Ends this ability on the target <see cref="AbilitySystem"/>
    //     /// </summary>
    //     /// <param name="abilitySystem">The target <see cref="AbilitySystem"/></param>
    //     void EndAbility(AbilitySystem abilitySystem);

    //     /// <summary>
    //     /// Activates this ability on the target <see cref="AbilitySystem"/>
    //     /// </summary>
    //     /// <param name="abilitySystem">The target <see cref="AbilitySystem"/></param>
    //     void ActivateAbility(AbilitySystem abilitySystem);

    //     /// <summary>
    //     /// Check if this ability can be activated by <see cref="AbilitySystem"/>
    //     /// </summary>
    //     /// <param name="abilitySystem">The target <see cref="AbilitySystem"/></param>
    //     /// <returns></returns>
    //     bool IsAbilityActivatable(AbilitySystem abilitySystem);

    //     /// <summary>
    //     /// Commits the <see cref="AbilitySystem"/> on the target <see cref="AbilitySystem"/>
    //     /// </summary>
    //     /// <param name="abilitySystem">The target <see cref="AbilitySystem"/></param>
    //     /// <returns></returns>
    //     bool CommitAbility(AbilitySystem abilitySystem);

    //     CoolDownInfo CalculateCooldown(AbilitySystem abilitySystem);
    // }
}
