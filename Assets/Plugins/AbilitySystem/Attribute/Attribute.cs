using GameplayAbilitySystem.Attributes;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes 
{
    [System.Serializable]
    public class Attribute
    {
        [SerializeField] private AttributeType _attributeType = default;
        [SerializeField] private float _baseValue = default;
        [SerializeField] private float _currentValue = default;

        public AttributeType AttributeType => _attributeType;
        public float BaseValue => _baseValue;
        public float CurrentValue => _currentValue;

        public Attribute(AttributeType type, float baseValue, float currentValue)
        {
            _attributeType = type;
            _baseValue = baseValue;
            _currentValue = currentValue;
        }

        public void SetBaseValue(AttributeSet set, ref float value) {
            set.PreBaseChange(this, ref value);
            _baseValue = value;
            set.AfterBaseChanged.Invoke(this);
        }

        public void SetCurrentValue(AttributeSet set, ref float value) {
            set.PreCurrentChange(this, ref value);
            _currentValue = value;
            set.AfterCurrentChanged.Invoke(this);
        }

    }
}
