using GameplayAbilitySystem.Attributes;
using GameplayAbilitySystem.Interfaces;
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

        public void SetCurrentValue(AttributeSet set, ref float value) {
            set.PreAttributeChange(this, ref value);
            _currentValue = value;
            set.AttributeCurrentValueChanged.Invoke(this);
        }

        public void SetBaseValue(AttributeSet set, ref float value) {
            set.PreAttributeBaseChange(this, ref value);
            _baseValue = value;
            set.AttributeBaseValueChanged.Invoke(this);
        }
    }
}
