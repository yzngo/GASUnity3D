using GAS.GameplayEffects;
using GAS.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GAS.Attributes {

    [Serializable]
    public class AttributeChangeEvent : UnityEvent<AttributeChangeData> {
    }
}
