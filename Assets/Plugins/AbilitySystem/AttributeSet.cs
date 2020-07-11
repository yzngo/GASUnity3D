using System.Collections.Generic;
using GameplayAbilitySystem.Effects;
using UnityEngine;
using GameplayAbilitySystem.Utility;

namespace GameplayAbilitySystem.Attributes 
{
    [AddComponentMenu("Ability System/Attribute Set")]
    public sealed class AttributeSet : MonoBehaviour
    {
        [SerializeField] private List<Attribute> attributes = default;
        public List<Attribute> Attributes => attributes;

        [SerializeField] private AttributeChangeHandler preBaseChangeHandler = default;
        [SerializeField] private AttributeChangeHandler preCurrentChangeHandler = default;
        private AttributeChangeEvent afterBaseChanged = new AttributeChangeEvent();
        private AttributeChangeEvent afterCurrentChanged = new AttributeChangeEvent();

        public AttributeChangeEvent AfterBaseChanged => afterBaseChanged;
        public AttributeChangeEvent AfterCurrentChanged => afterCurrentChanged;

        public void PreBaseChange(Attribute attribute, ref float newValue) => preBaseChangeHandler?.OnAttributeChange(this, attribute, ref newValue);
        public void PreCurrentChange(Attribute attribute, ref float newValue) => preCurrentChangeHandler?.OnAttributeChange(this, attribute, ref newValue);

        public void Add(AttributeType type, float baseValue, float currentValue)
        {
            if (attributes == null) {
                attributes = new List<Attribute>();
            }
            attributes.Add( new Attribute(type, baseValue, currentValue) );            
        }
    }
}
