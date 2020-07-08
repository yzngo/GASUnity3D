using System;
using GAS.Interfaces;
using GAS.GameplayEffects;
using UnityEngine;

namespace GAS {
    /// <inheritdoc />
    [Serializable]
    public class GameplayCost : IGameplayCost {
        /// <inheritdoc />
        public GameplayEffect CostGameplayEffect => _costGameplayEffect;

        /// <inheritdoc />
        [SerializeField] private GameplayEffect _costGameplayEffect = null;
    }
}
