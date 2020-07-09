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
        [SerializeField] private List<Attribute> attributes = default;
        public List<Attribute> Attributes => attributes;
        
        [SerializeField] private AttributeChangeHandler preBaseChangeHandler = default;
        [SerializeField] private AttributeChangeHandler preCurrentChangeHandler = default;
        [SerializeField] private AttributeChangeEvent afterBaseChanged = default;
        [SerializeField] private AttributeChangeEvent afterCurrentChanged = default;

        public AttributeChangeEvent AfterBaseChanged => afterBaseChanged;
        public AttributeChangeEvent AfterCurrentChanged => afterCurrentChanged;

        public void PreBaseChange(Attribute attribute, ref float newValue) => preBaseChangeHandler?.OnAttributeChange(this, attribute, ref newValue);
        public void PreCurrentChange(Attribute attribute, ref float newValue) => preCurrentChangeHandler?.OnAttributeChange(this, attribute, ref newValue);
    }
}
