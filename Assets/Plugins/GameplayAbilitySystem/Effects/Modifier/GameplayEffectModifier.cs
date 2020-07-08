using System;
using GAS.Attributes;
using GAS.Interfaces;
using UnityEngine;

namespace GAS.GameplayEffects {
    /// <inheritdoc />
    [Serializable]
    public class GameplayEffectModifier : IGameplayEffectModifier {
        [SerializeField] private AttributeType attributeType = null;

        [SerializeField] private ModifierOperationType modifierOperationType = default;

        [Space(10)]
        [SerializeField] private ModifierCalculationType magnitudeCalculationType = default;
        // Modification value for ScalableFloat type 
        [SerializeField] private float scaledMagnitude = 0f;

        [Space(10)]
        [SerializeField] private GameplayEffectModifierTagCollection sourceTags = null;

        [SerializeField] private GameplayEffectModifierTagCollection targetTags = null;

        /// <inheritdoc />
        public AttributeType Attribute => attributeType;
        /// <inheritdoc />
        public ModifierOperationType ModifierOperation => modifierOperationType;
        /// <inheritdoc />
        public float ScaledMagnitude => scaledMagnitude;
        /// <inheritdoc />
        public ModifierCalculationType ModifierCalculationType => magnitudeCalculationType;
        /// <inheritdoc />
        public GameplayEffectModifierTagCollection SourceTags => sourceTags;
        /// <inheritdoc />
        public GameplayEffectModifierTagCollection TargetTags => targetTags;

        /// <inheritdoc />
        public bool AttemptCalculateMagnitude(out float evaluatedMagnitude) {
            //TODO: PROPER IMPLEMENTATION
            evaluatedMagnitude = ScaledMagnitude;
            return true;
        }

        public GameplayEffectModifier InitializeEmpty() {
            this.attributeType = null;
            return this;
        }

    }
}