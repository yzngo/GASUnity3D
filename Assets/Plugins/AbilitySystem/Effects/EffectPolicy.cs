using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilitySystem.Effects {
    // Effect的策略集合
    // - 时长 (立即生效, 区间+magnitude, 无穷)
    // - 修改器
    [Serializable]
    public class EffectPolicy
    {
        [SerializeField] private DurationPolicy durationPolicy = default;
        [SerializeField] private float durationMagnitude = 0f;
        [SerializeField] private List<EffectModifier> modifiers = new List<EffectModifier>();

        public DurationPolicy DurationPolicy => durationPolicy;
        public float DurationValue => durationMagnitude;
        public List<EffectModifier> Modifiers => modifiers;
    }

}
