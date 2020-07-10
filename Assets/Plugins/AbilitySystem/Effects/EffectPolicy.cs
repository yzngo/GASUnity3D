using System;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem.Cues;

namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class EffectConfig 
    {
        public DurationConfig durationConfig;
        public PeriodConfig periodConfig;
        public StackConfig stackConfig;
        public List<EffectModifier> modifiers;
        public EffectTagContainer effectTags;
        public List<EffectCues> cues;
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
