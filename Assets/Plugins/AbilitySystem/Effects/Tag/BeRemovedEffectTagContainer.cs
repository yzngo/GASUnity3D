using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class RemovedEffectTagContainer
    {
        [SerializeField] private List<RemovedInfo> removed = new List<RemovedInfo>();
        public List<RemovedInfo> Removed => removed;

        // public bool HasAny(IEnumerable<GameplayTag> Tags) {
        //     return removed.Where(x => !Tags.Any(y => x.Tag == y)).Any();
        // }

        // public bool HasAll(IEnumerable<GameplayTag> Tags) {
        //     var addedTags = removed.Select(x => x.Tag);
        //     return !Tags.Except(addedTags).Any();
        // }
    }

    [Serializable]
    public class RemovedInfo {
        [Tooltip("GameplayEffects with this tag will be candidates for removal")]
        public GameplayTag Tag;
        
        [Tooltip("Number of stacks of each GameEffect to remove.  0 means remove all stacks.")]
        public int StacksToRemove = 0;
    }
}