using AbilitySystem.Attributes;
using AbilitySystem.Interfaces;
using UnityEngine;

namespace AbilitySystem.Attributes 
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
            set.AttributeCurrentValueChanged.Invoke(new AttributeChangeData()
            {
                Attribute = this
            });
        }

        public void SetBaseValue(IAttributeSet set, ref float value) {
            set.PreAttributeBaseChange(this, ref value);
            _baseValue = value;
            set.AttributeBaseValueChanged.Invoke(new AttributeChangeData()
            {
                Attribute = this
            });
        }
    }
}
