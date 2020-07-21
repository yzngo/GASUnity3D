using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class EffectModifier
    {
        [SerializeField] private string type = default;
        [SerializeField] private ModifierOperationType operationType = default;

        [FormerlySerializedAs("scaledValue")]
        [SerializeField] private float value = default;

        public string Type => type;
        public ModifierOperationType OperationType => operationType;
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
