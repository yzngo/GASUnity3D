using System;

namespace GameplayAbilitySystem.GameplayEffects {
    [Serializable]
    public class StackingPolicy {
        public StackingType StackingType;
        public int StackLimit;
        public StackRefreshPolicy StackDurationRefreshPolicy;   // 时间刷新策略
        public StackRefreshPolicy StackPeriodResetPolicy;       // 周期刷新策略
        public StackExpirationPolicy StackExpirationPolicy;     // 过期策略
    }

    public enum StackingType {
        None, 
        AggregatedBySource, 
        AggregatedByTarget
    }

    public enum StackRefreshPolicy {
        RefreshOnSuccessfulApplication,     // 成功施放之后刷新
        NeverRefresh                        // 永不刷新
    }

    public enum StackExpirationPolicy {
        ClearEntireStack,                       // 清空整个栈
        RemoveSingleStackAndRefreshDuration,    // 移除一个元素且刷新时间
        RefreshDuration                         // 刷新时间
    }
}

