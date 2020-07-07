using System;

namespace GAS.GameplayEffects {
    [Serializable]
    public class EffectPeriodicity {
        public float Period;
        public bool ExecuteOnApplication;
        public GameplayEffect ApplyGameEffectOnExecute;
    }

}

