using System;
using AbilitySystem.Interfaces;
using AbilitySystem.GameplayEffects;
using UnityEngine;

namespace AbilitySystem {
    [Serializable]
    public class GameplayCost : IGameplayCost {
        public GameplayEffect CostGameplayEffect => _costGameplayEffect;

        [SerializeField] private GameplayEffect _costGameplayEffect = null;
    }
}
