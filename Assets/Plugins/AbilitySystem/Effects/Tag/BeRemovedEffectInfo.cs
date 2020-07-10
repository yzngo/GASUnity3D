using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class BeRemovedEffectInfo {

        [Tooltip("GameplayEffects with this tag will be candidates for removal")]
        public GameplayTag EffectTag;
        
        [Tooltip("Number of stacks of each GameEffect to remove.  0 means remove all stacks.")]
        public int BeRemovedStacks = 0;
    }
}