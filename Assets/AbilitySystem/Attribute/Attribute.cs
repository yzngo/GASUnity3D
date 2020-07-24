using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem
{
    [System.Serializable]
    public class Attribute
    {
        private string attributeType;
        private float baseValue;
        private float currentValue;

        public string AttributeType => attributeType;
        public float BaseValue => baseValue;
        public float CurrentValue => currentValue;

        public Attribute(string type, float baseValue, float currentValue)
        {
            this.attributeType = type;
            this.baseValue = baseValue;
            this.currentValue = currentValue;
        }

        public void SetBaseValue(AttributeSet set, ref float value) {
            set.PreBaseChange(this, ref value);
            baseValue = value;
        }

        public void SetCurrentValue(AttributeSet set, ref float value) {
            set.PreCurrentChange(this, ref value);
            currentValue = value;
        }
    }
}