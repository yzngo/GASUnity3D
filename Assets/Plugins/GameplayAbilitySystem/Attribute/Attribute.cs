using GameplayAbilitySystem.Attributes;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes 
{
    [AddComponentMenu("Ability System/Attributes/Attribute")]
    [System.Serializable]
    public class Attribute : IAttribute {
        [SerializeField] AttributeType _attributeType = default;

        [SerializeField] float _baseValue = default;

        [SerializeField] float _currentValue = default;

        public float BaseValue => _baseValue;

        public float CurrentValue => _currentValue;

        public AttributeType AttributeType => _attributeType;

        public void SetCurrentValue(IAttributeSet set, ref float value) {
            set.PreAttributeChange(this, ref value);
            _currentValue = value;
            set.AttributeCurrentValueChanged.Invoke(this);
        }

        public void SetBaseValue(IAttributeSet set, ref float value) {
            set.PreAttributeBaseChange(this, ref value);
            _baseValue = value;
            set.AttributeBaseValueChanged.Invoke(this);
        }
    }
}
