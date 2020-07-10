using System;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem.Cues;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class EffectConfigs 
    {
        public DurationConfig DurationConfig;
        public PeriodConfig PeriodConfig;
        public StackConfig StackConfig;
        public List<EffectModifier> Modifiers;
        public EffectTagContainer EffectTags;
        [FormerlySerializedAs("cues")]
        public List<EffectCues> Cues;
    }

    [Serializable]
    public class DurationConfig {
        public DurationPolicy policy;
        public float durationLength;
    }





    [Serializable]
    public class EffectPolicy
    {
        [SerializeField] private DurationPolicy durationPolicy = default;
        [SerializeField] private float durationMagnitude = 0f;
        [SerializeField] private List<EffectModifier> modifiers = new List<EffectModifier>();

        public DurationPolicy DurationPolicy => durationPolicy;
        public float DurationValue => durationMagnitude;
        public List<EffectModifier> Modifiers => modifiers;
    }
}
