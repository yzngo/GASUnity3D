using System;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem.Cues;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class EffectConfigs {
        public EffectType EffectType;
        public DurationConfig DurationConfig;
        public PeriodConfig PeriodConfig;
        public StackConfig StackConfig;
        public List<EffectModifier> Modifiers;
        public EffectTagContainer EffectTags;
        public List<EffectCues> Cues;
    }
// Type ----------------------------------------------------------------------------------
    [Serializable]
    public enum EffectType {
        Normal,
        Cost,
        CoolDown
    }
// Duration ------------------------------------------------------------------------------

    [Serializable]
    public class DurationConfig {
        public DurationPolicy Policy;
        public float DurationLength;
    }

    public enum DurationPolicy {
        Instant,
        Duration,
        Infinite
    }

// Period --------------------------------------------------------------------------------

    [Serializable]
    public class PeriodConfig {
        public float Period;
        public bool IsExecuteOnApply;
        public Effect EffectOnExecute;
    }

// Stack ---------------------------------------------------------------------------------

    [Serializable]
    public class StackConfig {
        public StackType Type;
        public int Limit;
        public StackRefreshPolicy DurationRefreshPolicy;   // 时间刷新策略
        public StackRefreshPolicy PeriodResetPolicy;       // 周期刷新策略
        public StackExpirationPolicy ExpirationPolicy;     // 过期策略
    }

    public enum StackType {
        None, 
        StackBySource, 
        StackByTarget
    }

    public enum StackRefreshPolicy {
        RefreshOnSuccessfulApply,     // 成功施放之后刷新
        NeverRefresh                        // 永不刷新
    }

    public enum StackExpirationPolicy {
        ClearEntireStack,                       // 清空整个栈
        RemoveSingleStackAndRefreshDuration,    // 移除一个元素且刷新时间
        RefreshDuration                         // 刷新时间
    }
}
