using System;

namespace GAS.GameplayEffects {
    [Serializable]
    public class StackingPolicy {
        public StackingType StackingType;
        public int StackLimit;
        public StackRefreshPolicy StackDurationRefreshPolicy;
        public StackRefreshPolicy StackPeriodResetPolicy;
        public StackExpirationPolicy StackExpirationPolicy;
    }

    public enum StackingType {
        None, 
        AggregatedBySource, 
        AggregatedByTarget
    }

    public enum StackRefreshPolicy {
        RefreshOnSuccessfulApplication, NeverRefresh
    }

    public enum StackExpirationPolicy {
        ClearEntireStack, RemoveSingleStackAndRefreshDuration, RefreshDuration
    }
}

