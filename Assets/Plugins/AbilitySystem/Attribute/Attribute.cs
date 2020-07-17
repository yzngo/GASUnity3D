using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem.Attributes 
{
    [System.Serializable]
    public class Attribute
    {
        [SerializeField] private AttributeType attributeType = default;
        [SerializeField] private float baseValue = default;
        [SerializeField] private float currentValue = default;

        public AttributeType AttributeType => attributeType;
        public float BaseValue => baseValue;
        public float CurrentValue => currentValue;

        public Attribute(AttributeType type, float baseValue, float currentValue)
        {
            this.attributeType = type;
            this.baseValue = baseValue;
            this.currentValue = currentValue;
        }

        public void SetBaseValue(AttributeSet set, ref float value) {
            set.PreBaseChange(this, ref value);
            baseValue = value;
            set.AfterBaseChanged.Invoke(this);
        }

        public void SetCurrentValue(AttributeSet set, ref float value) {
            set.PreCurrentChange(this, ref value);
            currentValue = value;
            set.AfterCurrentChanged.Invoke(this);
        }

    }
}
