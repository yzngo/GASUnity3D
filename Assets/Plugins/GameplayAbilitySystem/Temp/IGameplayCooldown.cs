using System.Collections.Generic;
using GameplayAbilitySystem.Effects;

namespace GameplayAbilitySystem.Interfaces {
    /// <summary>
    /// This is used to represent cooldowns for a <see cref="GameplayAbility"/>.  
    /// Multiple <see cref="IGameplayCooldown"/> can be set for an ability to represent multiple
    /// shared cooldowns (e.g. a global cooldown shared by all abilities, and a <see cref="GameplayAbility"/> specific cooldown)
    /// </summary>
    public interface IGameplayCooldown {
        /// <summary>
        /// The descriptor for the cooldown.  The tags granted by this are used to determine cooldowns
        /// </summary>
        /// <value></value>
        Effect CooldownGameplayEffect { get; }

        /// <summary>
        /// Gets the list of cooldown tags that this applies
        /// </summary>
        /// <returns></returns>
        IEnumerable<GameplayTag> GetCooldownTags();
    }
}
