using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class GameplayEffectAddRemoveTagContainer
    {

        [SerializeField]
        List<GameplayTag> _added = new List<GameplayTag>();

        public List<GameplayTag> Added => _added;

        // public bool HasAny(IEnumerable<GameplayTag> Tags) {
        //     return _added.Where(x => !Tags.Any(y => x == y)).Any();
        // }

        // public bool HasAll(IEnumerable<GameplayTag> Tags) {
        //     return !Tags.Except(_added).Any();
        // }

    }
}