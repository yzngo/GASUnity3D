using System;
using GAS.Interfaces;
using GAS.GameplayEffects;
using UnityEngine;

namespace GAS {
    [Serializable]
    public class GameplayCost : IGameplayCost {
        public GameplayEffect CostGameplayEffect => _costGameplayEffect;

        [SerializeField] private GameplayEffect _costGameplayEffect = null;
    }
}
