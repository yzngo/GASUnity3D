using System;
using System.Collections.Generic;
using System.Linq;
using GAS;
using GAS.Interfaces;
using UnityEngine;

namespace GAS.GameplayEffects {

    [Serializable]
    public class GameplayEffectAddRemoveTagContainer : GameplayEffectTagContainer, IAddedRemovedTags {
        [SerializeField]
        List<GameplayTag> _added = new List<GameplayTag>();

        public List<GameplayTag> Added => _added;

        public override bool HasAny(IEnumerable<GameplayTag> Tags) {
            return _added.Where(x => !Tags.Any(y => x == y)).Any();
        }

        public override bool HasAll(IEnumerable<GameplayTag> Tags) {
            return !Tags.Except(_added).Any();
        }

    }
}