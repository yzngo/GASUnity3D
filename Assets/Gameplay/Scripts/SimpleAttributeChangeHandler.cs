using System.Linq;
using GameplayAbilitySystem.Attributes;
using UnityEngine;


namespace AbilitySystemDemo.Attributes {
    [CreateAssetMenu(fileName = "Simple Attribute Change Handler", menuName = "Ability System Demo/Attributes/Simple Attribute Change Handler")]
    public class SimpleAttributeChangeHandler : AttributeChangeHandler {
        public AttributeType MaxHealth;
        public AttributeType Health;
        public AttributeType MaxMana;
        public AttributeType Mana;

        public override void OnAttributeChange(AttributeSet attributeSet, Attribute attribute, ref float value) {
            
            if (attribute.AttributeType == Health) {
                HandleHealthChange(ref value, attributeSet.Attributes.First(x => x.AttributeType == MaxHealth).CurrentValue);
            } else if (attribute.AttributeType == Mana) {
                HandleManaChange(ref value, attributeSet.Attributes.First(x => x.AttributeType == MaxMana).CurrentValue);
            }
        }

        private void HandleHealthChange(ref float value, float maxValue) {
            value = Mathf.Clamp(value, 0, maxValue);
        }

        private void HandleManaChange(ref float value, float maxValue) {
            value = Mathf.Clamp(value, 0, maxValue);
        }


    }
}
