using System;
using UnityEngine.Events;
using System.Collections.Generic;

namespace GameplayAbilitySystem.Abilities 
{
    [Serializable]
    public class AbilityEvent : UnityEvent<AbilityEventData> 
    {
    }

    [Serializable]
    public struct AbilityEventData 
    {
        public GameplayTag AbilityTag;
        // public AbilitySystem Instigator;
        public AbilitySystem Target;
        // public object OptionalObject;
        // Gameplay Effect handle?
        // public List<GameplayTag> InstigatorTags;
        // public List<GameplayTag> TargetTags;
        // public float EventMagnitude;
        // Target Data?
    }
}