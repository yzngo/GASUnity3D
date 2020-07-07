namespace GameplayAbilitySystem.Enums {
    //根据不同规则产生float值给modifier使用,使其基于选定的operation改变指定的attribute
    public enum ModifierCalculationType {
        ScalableFloat,
        AttributeBased, 
        CustomCalculationClass, 
        SetByCaller
    }
}
