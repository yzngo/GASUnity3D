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

        public AttributeType AttributeType => attributeType;
        public ModifierOperationType OperationType => operationType;
        public float ScaledMagnitude => scaledValue;
        public ModifierCalculationType ModifierCalculationType => valueSourceType;
        
        public bool AttemptCalculateMagnitude(out float evaluatedValue) {
            evaluatedValue = ScaledMagnitude;
            return true;
        }

        public EffectModifier InitializeEmpty() {
            this.attributeType = null;
            return this;
        }

    }
}
