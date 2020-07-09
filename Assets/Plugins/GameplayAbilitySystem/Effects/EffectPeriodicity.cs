using System;

namespace GameplayAbilitySystem.GameplayEffects {

    // 周期性的effect的配置项
    // 例如DOT(damage over time)类型的effect可以应用period
    [Serializable]
    public class EffectPeriodicity {
        public float Period;        // 周期时间
        public bool ExecuteOnApplication;   // 应用时即执行
        public GameplayEffect EffectOnExecute;     //执行时应用的effect
    }
}

