using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class EffectModifier
    {
        public string Type;
        public OperationType OperationType;
        public float Value;

    }
    
    public enum OperationType 
    {
        Add, 
        Multiply, 
        Divide,
        Override
    }
}
