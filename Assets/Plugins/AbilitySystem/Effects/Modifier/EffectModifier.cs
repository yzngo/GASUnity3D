using System;
using GameplayAbilitySystem.Attributes;
using UnityEngine;

namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class EffectModifier
    {
        [SerializeField] private AttributeType attributeType = null;

        [SerializeField] private ModifierOperationType modifierOperationType = default;

        [Space(10)]
        // todo -> to ValueSourceType
        [SerializeField] private ModifierCalculationType magnitudeCalculationType = default;
        // Modification value for ScalableFloat type 
        // todo -> to scaledValue
        [SerializeField] private float scaledMagnitude = 0f;

        // [Space(10)]

        // [SerializeField] private GameplayEffectModifierTagCollection sourceTags = null;

        // [SerializeField] private GameplayEffectModifierTagCollection targetTags = null;

        
        public AttributeType AttributeType => attributeType;
        
        public ModifierOperationType ModifierOperation => modifierOperationType;
        
        public float ScaledMagnitude => scaledMagnitude;
        
        public ModifierCalculationType ModifierCalculationType => magnitudeCalculationType;
        
        // public GameplayEffectModifierTagCollection SourceTags => sourceTags;
        
        // public GameplayEffectModifierTagCollection TargetTags => targetTags;

        public bool AttemptCalculateMagnitude(out float evaluatedValue) {
            //TODO: PROPER IMPLEMENTATION
            evaluatedValue = ScaledMagnitude;
            return true;
        }

        public EffectModifier InitializeEmpty() {
            this.attributeType = null;
            return this;
        }

    }
}
