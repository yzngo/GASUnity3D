using AbilitySystem.GameplayEffects;
using AbilitySystem.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace AbilitySystem.Attributes {

    [Serializable]
    public class AttributeChangeEvent : UnityEvent<AttributeChangeData> {
    }
}
