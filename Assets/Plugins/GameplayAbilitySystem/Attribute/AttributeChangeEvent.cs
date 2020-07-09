using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GameplayAbilitySystem.Attributes {

    [Serializable]
    public class AttributeChangeEvent : UnityEvent<Attribute> {
    }
}
