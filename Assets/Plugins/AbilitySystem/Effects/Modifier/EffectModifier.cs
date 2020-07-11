using System;
using GameplayAbilitySystem.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class EffectModifier
    {
        [SerializeField] private AttributeType attributeType = default;
        [SerializeField] private ModifierOperationType operationType = default;
        [SerializeField] private ModifierCalculationType valueSourceType = default;
        [SerializeField] private float scaledValue = default;

        // [Space(10)]
        // [SerializeField] private GameplayEffectModifierTagCollection sourceTags = null;
        // [SerializeField] private GameplayEffectModifierTagCollection targetTags = null;
        
        public AttributeType AttributeType => attributeType;
        public ModifierOperationType OperationType => operationType;
        public float ScaledMagnitude => scaledValue;
        public ModifierCalculationType ModifierCalculationType => valueSourceType;
        
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
