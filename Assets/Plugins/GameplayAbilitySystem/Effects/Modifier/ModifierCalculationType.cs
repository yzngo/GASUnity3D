namespace GameplayAbilitySystem.Effects {
    //根据不同规则产生float值给modifier使用,使其基于选定的operation改变指定的attribute
    public enum ModifierCalculationType {
        ScalableFloat,  //按照表格读取技能相应等级对应的float值, 只有一个值则相当于硬编码float值
        AttributeBased, //从attribute中读取值
        CustomCalculationClass, 
        SetByCaller     //运行时由caller设置
    }
}
