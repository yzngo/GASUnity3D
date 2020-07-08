using GAS.Attributes;
using GAS.Interfaces;
using UnityEngine;

namespace GAS.Attributes 
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

        public void SetAttributeCurrentValue(IAttributeSet attributeSet, ref float newValue) {
            attributeSet.PreAttributeChange(this, ref newValue);
            _currentValue = newValue;
            attributeSet.AttributeCurrentValueChanged.Invoke(new AttributeChangeData()
            {
                Attribute = this
            });
        }

        public void SetAttributeBaseValue(IAttributeSet attributeSet, ref float newValue) {
            attributeSet.PreAttributeBaseChange(this, ref newValue);
            _baseValue = newValue;
            attributeSet.AttributeBaseValueChanged.Invoke(new AttributeChangeData()
            {
                Attribute = this
            });
        }
    }
}
