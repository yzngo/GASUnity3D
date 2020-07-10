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
        [FormerlySerializedAs("policy")]
        public DurationPolicy Policy;
        [FormerlySerializedAs("durationLength")]
        public float DurationLength;
    }
}