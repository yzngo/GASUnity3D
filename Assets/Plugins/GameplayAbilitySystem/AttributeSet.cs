using System.Collections.Generic;
using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes 
{
    [AddComponentMenu("Ability System/Attribute Set")]
    [RequireComponent(typeof(AbilitySystem))]
    public sealed class AttributeSet : MonoBehaviour
    {
        [SerializeField] private AttributeChangeEvent attributeBaseValueChanged = default;
        [SerializeField] private AttributeChangeEvent attributeCurrentValueChanged = default;
        [SerializeField] private List<Attribute> attributes = default;

        public AttributeChangeEvent AttributeBaseValueChanged => attributeBaseValueChanged;
        public AttributeChangeEvent AttributeCurrentValueChanged => attributeCurrentValueChanged;
        public List<Attribute> Attributes => attributes;

        [SerializeField] private AttributeChangeHandler preAttributeBaseChangeHandler = default;
        public AttributeChangeHandler PreAttributeBaseChangeHandler => preAttributeBaseChangeHandler;

        [SerializeField] private AttributeChangeHandler preAttributeChangeHandler = default;
        public AttributeChangeHandler PreAttributeChangeHandler => preAttributeChangeHandler;

        public void PreAttributeBaseChange(Attribute attribute, ref float newValue) {
            if (preAttributeBaseChangeHandler != null) {
                preAttributeBaseChangeHandler.OnAttributeChange(this, attribute, ref newValue);
            }
            return;
        }

        public void PreAttributeChange(Attribute attribute, ref float newValue) {
            if (preAttributeChangeHandler != null) {
                preAttributeChangeHandler.OnAttributeChange(this, attribute, ref newValue);
            }
            return;
        }
    }
}
