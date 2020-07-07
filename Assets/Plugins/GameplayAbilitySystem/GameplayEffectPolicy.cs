using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Enums;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayEffects {
    [Serializable]
    public class GameplayEffectPolicy : IGameplayEffectPolicy {
        [SerializeField]
        private DurationPolicy durationPolicy = default;

        [SerializeField]
        private float durationMagnitude = 0f;

        [SerializeField]
        private List<GameplayEffectModifier> _modifiers = new List<GameplayEffectModifier>();

        public DurationPolicy DurationPolicy => durationPolicy;
        public float DurationMagnitude => durationMagnitude;
        public List<GameplayEffectModifier> Modifiers => _modifiers;
    }

}
