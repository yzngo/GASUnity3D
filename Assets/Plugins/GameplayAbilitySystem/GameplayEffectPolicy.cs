using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Enums;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayEffects {
    // Effect的策略集合
    // - 时长策略 (立即生效, 区间+magnitude, 无穷)
    [Serializable]
    public class GameplayEffectPolicy : IGameplayEffectPolicy {
        [SerializeField]
        private DurationPolicy durationPolicy = default;

        [SerializeField]
        private float durationMagnitude = 0f;

        [SerializeField]
        private List<GameplayEffectModifier> modifiers = new List<GameplayEffectModifier>();

        public DurationPolicy DurationPolicy => durationPolicy;
        public float DurationMagnitude => durationMagnitude;
        public List<GameplayEffectModifier> Modifiers => modifiers;
    }

}
