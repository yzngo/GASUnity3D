using System;
using System.Collections.Generic;
using AbilitySystem.Interfaces;
using UnityEngine;

namespace AbilitySystem.GameplayEffects {
    [Serializable]
    public class GameplayEffectRequireIgnoreTagContainer : GameplayEffectTagContainer, IRequireIgnoreTags {
        [SerializeField]
        private List<GameplayTag> requirePresence = new List<GameplayTag>();
        
        [SerializeField]
        private List<GameplayTag> requireAbsence = new List<GameplayTag>();

        public List<GameplayTag> RequirePresence => requirePresence;

        public List<GameplayTag> RequireAbsence => requireAbsence;

        public override bool HasAll(IEnumerable<GameplayTag> Tags) {
            throw new System.NotImplementedException();
        }

        public override bool HasAny(IEnumerable<GameplayTag> Tags) {
            throw new System.NotImplementedException();
        }
    }
}