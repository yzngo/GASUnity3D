using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameplayAbilitySystem.Utility;

namespace GameplayAbilitySystem.Attributes 
{
    [AddComponentMenu("Ability System/Attribute Set")]
    public sealed class AttributeSet : MonoBehaviour
    {
        private List<Attribute> attributes = new List<Attribute>();
        public List<Attribute> Attributes => attributes;

        private AttributeChangeEvent afterBaseChanged = new AttributeChangeEvent();
        private AttributeChangeEvent afterCurrentChanged = new AttributeChangeEvent();

        public void PreBaseChange(Attribute attribute, ref float newValue)
        {
            OnAttributeChange(attribute, ref newValue);
        }

        public void PreCurrentChange(Attribute attribute, ref float newValue)
        {
            OnAttributeChange(attribute, ref newValue);
        }

        public void Add(string type, float baseValue, float currentValue)
        {
            // if (attributes == null) {
            //     attributes = new List<Attribute>();
            // }
            attributes.Add( new Attribute(type, baseValue, currentValue) );            
        }

        public void OnAttributeChange(Attribute attribute, ref float value) 
        {
            if (attribute.AttributeType == AttributeType.Health) {
                value = Mathf.Clamp(value, 0, Attributes.First(x => x.AttributeType == AttributeType.MaxHealth).CurrentValue);

            } else if (attribute.AttributeType == AttributeType.Mana) {
                value = Mathf.Clamp(value, 0, Attributes.First(x => x.AttributeType == AttributeType.MaxMana).CurrentValue);
            }
        }
    }
}
