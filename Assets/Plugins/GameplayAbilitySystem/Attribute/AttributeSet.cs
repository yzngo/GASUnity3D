using System.Collections.Generic;
using GAS.GameplayEffects;
using GAS.Interfaces;
using UnityEngine;

namespace GAS.Attributes {

    /// <inheritdoc />
    [AddComponentMenu("Gameplay Ability System/Attribute Set")]
    [System.Serializable]
    [RequireComponent(typeof(AbilitySystemComponent))]
    public sealed class AttributeSet : MonoBehaviour, IAttributeSet {
        [SerializeField] private AttributeChangeEvent attributeBaseValueChanged = default;
        [SerializeField] private AttributeChangeEvent attributeCurrentValueChanged = default;
        [SerializeField] private List<Attribute> attributes;

        /// <inheritdoc />
        public AttributeChangeEvent AttributeBaseValueChanged => attributeBaseValueChanged;

        /// <inheritDoc />
        public AttributeChangeEvent AttributeCurrentValueChanged => attributeCurrentValueChanged;

        /// <inheritdoc />
        public List<Attribute> Attributes { 
            get => attributes; 
            set => attributes = value; 
        }

        [SerializeField] private BaseAttributeChangeHandler preAttributeBaseChangeHandler = default;
        public BaseAttributeChangeHandler PreAttributeBaseChangeHandler => preAttributeBaseChangeHandler;

        [SerializeField] private BaseAttributeChangeHandler preAttributeChangeHandler = default;
        public BaseAttributeChangeHandler PreAttributeChangeHandler => preAttributeChangeHandler;

        /// <inheritdoc />
        public AbilitySystemComponent GetOwningAbilitySystem() {
            return this.GetComponent<AbilitySystemComponent>();
        }

        /// <inheritdoc />
        // public bool PreGameplayEffectExecute(GameplayEffect effect, GameplayModifierEvaluatedData evalData) {
        //     return true;
        // }

        /// <inheritdoc />
        public void PreAttributeBaseChange(IAttribute attribute, ref float newMagnitude) {
            if (preAttributeBaseChangeHandler != null) {
                preAttributeBaseChangeHandler.OnAttributeChange(this, attribute, ref newMagnitude);
            }
            return;
        }

        /// <inheritdoc />
        public void PreAttributeChange(IAttribute attribute, ref float newValue) {
            if (preAttributeChangeHandler != null) {
                preAttributeChangeHandler.OnAttributeChange(this, attribute, ref newValue);
            }
            return;
        }

        /// <inheritdoc />
        // public void PostGameplayEffectExecute(GameplayEffect effect, GameplayModifierEvaluatedData evalData) {
        //     return;
        // }
    }
}
