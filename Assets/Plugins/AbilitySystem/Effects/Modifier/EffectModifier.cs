using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class EffectModifier
    {
        [SerializeField] private string type = default;
        [SerializeField] private OperationType operationType = default;
        [SerializeField] private float value = default;

        public string Type => type;
        public OperationType OperationType => operationType;
        public float Value => value;
        
        public bool AttemptCalculateMagnitude(out float evaluatedValue) {
            evaluatedValue = Value;
            return true;
        }

        public EffectModifier InitializeEmpty() {
            this.type = string.Empty;
            return this;
        }

    }
}
