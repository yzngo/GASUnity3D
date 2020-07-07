using System.Collections.Generic;
using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes {

    /// <inheritdoc />
    [AddComponentMenu("Gameplay Ability System/Attribute Set")]
    [System.Serializable]
    [RequireComponent(typeof(AbilitySystemComponent))]
    public class AttributeSet : MonoBehaviour, IAttributeSet {
        [SerializeField]
        private AttributeChangeDataEvent attributeBaseValueChanged = default;
        /// <inheritdoc />
        public AttributeChangeDataEvent AttributeBaseValueChanged => attributeBaseValueChanged;

        /// <inheritDoc />
        [SerializeField]
        private AttributeChangeDataEvent attributeCurrentValueChanged = default;
        public AttributeChangeDataEvent AttributeCurrentValueChanged => attributeCurrentValueChanged;

        /// <inheritdoc />
        [SerializeField]
        private List<Attribute> attributes;
        public List<Attribute> Attributes { get => attributes; set => attributes = value; }

        [SerializeField]
        private BaseAttributeChangeHandler preAttributeBaseChangeHandler = default;
        public BaseAttributeChangeHandler PreAttributeBaseChangeHandler => preAttributeBaseChangeHandler;

        [SerializeField]
        private BaseAttributeChangeHandler preAttributeChangeHandler = default;
        public BaseAttributeChangeHandler PreAttributeChangeHandler => preAttributeChangeHandler;

        /// <inheritdoc />
        public AbilitySystemComponent GetOwningAbilitySystem() {
            return this.GetComponent<AbilitySystemComponent>();
        }

        /// <inheritdoc />
        public bool PreGameplayEffectExecute(GameplayEffect Effect, GameplayModifierEvaluatedData EvalData) {
            return true;
        }

        /// <inheritdoc />
        public void PreAttributeBaseChange(IAttribute Attribute, ref float newMagnitude) {
            if (preAttributeBaseChangeHandler != null) {
                preAttributeBaseChangeHandler.OnAttributeChange(this, Attribute, ref newMagnitude);
            }
            return;
        }

        /// <inheritdoc />
        public void PreAttributeChange(IAttribute Attribute, ref float NewValue) {
            if (preAttributeChangeHandler != null) {
                preAttributeChangeHandler.OnAttributeChange(this, Attribute, ref NewValue);
            }
            return;
        }

        /// <inheritdoc />
        public void PostGameplayEffectExecute(GameplayEffect Effect, GameplayModifierEvaluatedData EvalData) {
            return;
        }
    }
}
