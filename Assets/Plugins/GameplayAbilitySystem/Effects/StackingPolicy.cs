using System;

namespace GAS.GameplayEffects {
    [Serializable]
    public class StackingPolicy {
        public StackingType StackingType;
        public int StackLimit;
        public StackRefreshPolicy StackDurationRefreshPolicy;   // 时间差值
        public StackRefreshPolicy StackPeriodResetPolicy;       // 更长的时间段
        public StackExpirationPolicy StackExpirationPolicy;
    }

    public enum StackingType {
        None, 
        AggregatedBySource, 
        AggregatedByTarget
    }

    public enum StackRefreshPolicy {
        RefreshOnSuccessfulApplication, 
        NeverRefresh
    }

    public enum StackExpirationPolicy {
        ClearEntireStack, 
        RemoveSingleStackAndRefreshDuration, 
        RefreshDuration
    }
}

