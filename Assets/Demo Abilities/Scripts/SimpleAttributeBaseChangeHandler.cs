using System.Linq;
using GameplayAbilitySystem.Attributes;
using UnityEngine;


namespace AbilitySystemDemo.Attributes {
    [CreateAssetMenu(fileName = "Simple Attribute Base Change Handler", menuName = "Ability System Demo/Attributes/Simple Attribute Base Change Handler")]
    public class SimpleAttributeBaseChangeHandler : AttributeChangeHandler {
        public AttributeType MaxHealth;
        public AttributeType Health;
        public AttributeType MaxMana;
        public AttributeType Mana;

        public override void OnAttributeChange(AttributeSet attributeSet, Attribute attribute, ref float Value) {
            if (attribute.AttributeType == Health) {
                HandleHealthChange(ref Value, attributeSet.Attributes.First(x => x.AttributeType == MaxHealth).CurrentValue);
            } else if (attribute.AttributeType == Mana) {
                HandleManaChange(ref Value, attributeSet.Attributes.First(x => x.AttributeType == MaxMana).CurrentValue);
            }
        }

        private void HandleHealthChange(ref float Value, float maxValue) {
            Value = Mathf.Clamp(Value, -Mathf.Infinity, maxValue);
        }

        private void HandleManaChange(ref float Value, float maxValue) {
            Value = Mathf.Clamp(Value, -Mathf.Infinity, maxValue);
        }


    }
}
