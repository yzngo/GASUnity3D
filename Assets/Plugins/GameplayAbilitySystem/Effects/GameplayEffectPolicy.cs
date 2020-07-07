using System;
using System.Collections.Generic;
using GAS.Enums;
using GAS.Interfaces;
using UnityEngine;

namespace GAS.GameplayEffects {
    // Effect的策略集合
    // - 时长 (立即生效, 区间+magnitude, 无穷)
    // - 修改器
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
