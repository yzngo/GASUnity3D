using System;

namespace GameplayAbilitySystem.Effects 
{
    // 每[Period]时间执行一次[EffectOnExecute]  DOT, HOT类型
    [Serializable]
    public class PeriodPolicy 
    {
        public float Period;        // 周期时间
        public bool IsExecuteOnApply;   // 应用时即执行
        public Effect EffectOnExecute;     //执行时应用的effect
    }
}

