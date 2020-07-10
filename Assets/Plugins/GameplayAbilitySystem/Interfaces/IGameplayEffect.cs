using System.Linq;
using System.Collections.Generic;
using GameplayAbilitySystem.Effects;

namespace GameplayAbilitySystem.Interfaces
{

    /// <summary>
    /// This is used to define how long the <see cref="Effect"/> lasts, and what it does (e.g. how much damage)
    /// </summary>
    public interface IGameplayEffectPolicy
    {
        /// <summary>
        /// Whether the <see cref="Effect"/> lasts for some finite time, infinite time, or is applied instantly
        /// </summary>
        /// <value></value>
        DurationPolicy DurationPolicy { get; }

        /// <summary>
        /// Duration of the <see cref="Effect"/>, if this lasts for some finite time
        /// </summary>
        /// <value></value>
        float DurationValue { get; }

        /// <summary>
        /// How the <see cref="Effect"/> affects attributes
        /// </summary>
        /// <value></value>
        List<GameplayEffectModifier> Modifiers { get; }
    }

    /// <summary>
    /// Gameplay effect tags
    /// </summary>
    public interface IGameplayEffectTags
    {
        /// <summary>
        /// <see cref="GameplayTag"/> the <see cref="Effect"/> has
        /// </summary>
        /// <value></value>
        GameplayEffectAddRemoveTagContainer EffectTags { get; }

        /// <summary>
        /// <see cref="GameplayTag"/> that are given to the <see cref="AbilitySystem"/>
        /// </summary>
        /// <value></value>
        GameplayEffectAddRemoveTagContainer GrantedToASCTags { get; }

        /// <summary>
        /// <see cref="GameplayTag"/> that are required on the <see cref="AbilitySystem"/> for the <see cref="Effect"> to have an effect.  
        /// If these <see cref="GameplayTag"/> are not on the <see cref="AbilitySystem"/>, the effect is "disabled" until these <see cref="GameplayTag"/> are present.
        /// </summary>
        /// <value></value>
        GameplayEffectRequireIgnoreTagContainer OngoingRequiredTags { get; }

        /// <summary>
        /// These <see cref="GameplayTag"/> are required on the target to apply the <see cref="Effect"/>.  Once the <see cref="Effect"/> is applied,
        /// this has no effect.
        /// </summary>
        /// <value></value>
        GameplayEffectRequireIgnoreTagContainer ApplyRequiredTags { get; }

        /// <summary>
        /// Removes any existing <see cref="Effect"/> that have these <see cref="GameplayTag"/>.
        /// </summary>
        /// <value></value>
        GameplayEffectAddRemoveStacksTagContainer BeRemovedEffectsTags { get; }
    }

}
