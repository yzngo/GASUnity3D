using System;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem.Effects {
    [Serializable]
    public class StackPolicy {
        [FormerlySerializedAs("StackType")]
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

