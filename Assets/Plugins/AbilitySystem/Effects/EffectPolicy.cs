using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.Effects {
    // Effect的策略集合
    // - 时长 (立即生效, 区间+magnitude, 无穷)
    // - 修改器
    [Serializable]
    public class EffectPolicy
    {
        [Tooltip(
@"游戏效果策略 
        Instant  执行一次  
        Duration 执行一段时间
        Infinite 一直执行不停止")]
        [SerializeField] private DurationPolicy durationPolicy = default;

        [SerializeField] private float durationMagnitude = 0f;

        [SerializeField] private List<EffectModifier> modifiers = new List<EffectModifier>();

        public DurationPolicy DurationPolicy => durationPolicy;
        public float DurationValue => durationMagnitude;
        public List<EffectModifier> Modifiers => modifiers;
    }

}
