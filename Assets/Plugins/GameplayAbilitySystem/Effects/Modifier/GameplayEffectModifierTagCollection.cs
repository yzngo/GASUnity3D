
using System;
using System.Collections.Generic;
using GAS;
using GAS.Attributes;

namespace GAS.GameplayEffects {

    /// <summary>
    /// Defines tags that must be present/must not be present on a <see cref="AbilitySystemComponent"/>
    /// </summary>
    [Serializable]
    public class GameplayEffectModifierTagCollection {
        /// <summary>
        /// <see cref="GameplayTag"/> that must be present on the <see cref="AbilitySystemComponent"/>
        /// </summary>
        public List<GameplayTag> RequireTags;

        /// <summary>
        /// <see cref="GameplayTag"/> that must not be present on the <see cref="AbilitySystemComponent"/>
        /// </summary>        
        public List<GameplayTag> IgnoreTags;
    }
}
