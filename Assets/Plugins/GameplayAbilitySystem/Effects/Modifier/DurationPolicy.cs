namespace GameplayAbilitySystem.GameplayEffects {
    public enum DurationPolicy {
        Instant,    // 永久修改basevalue
        Duration,    // 临时修改currentValue
        Infinite    // 临时修改currentValue
    }

}
