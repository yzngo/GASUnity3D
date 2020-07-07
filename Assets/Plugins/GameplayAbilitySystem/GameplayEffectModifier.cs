using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Statics;
using GameplayAbilitySystem.Attributes;
using GameplayAbilitySystem.Enums;
using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilitySystem {
    /// <inheritdoc />
    [Serializable]
    public class GameplayEffectModifier : IGameplayEffectModifier {
        [SerializeField]
        private AttributeType attributeType = null;

        [SerializeField]
        private ModifierOperationType modifierOperationType = default;
        [Space(10)]

        [SerializeField]
        private ModifierCalculationType magnitudeCalculationType = default;

        [SerializeField]
        private float scaledMagnitude = 0f;
        [Space(10)]

        [SerializeField]
        private GameplayEffectModifierTagCollection sourceTags = null;

        [SerializeField]
        private GameplayEffectModifierTagCollection targetTags = null;

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
        public bool AttemptCalculateMagnitude(out float EvaluatedMagnitude) {
            //TODO: PROPER IMPLEMENTATION
            EvaluatedMagnitude = this.ScaledMagnitude;
            return true;
        }

        public GameplayEffectModifier InitialiseEmpty() {
            this.attributeType = null;
            return this;
        }

    }
}
