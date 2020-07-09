using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GameplayAbilitySystem.Attributes {

    /// <summary>
    /// Container for attribute change events
    /// </summary>
    [Serializable]
    public struct AttributeChangeData {

        public IAttribute Attribute;


    }

}
