using GameplayAbilitySystem.Interfaces;
using UnityEngine;


namespace GameplayAbilitySystem.Attributes {
    public class BaseAttributeChangeHandler : ScriptableObject {

        public virtual void OnAttributeChange(IAttributeSet AttributeSet, IAttribute Attribute, ref float Value) {

        }
    }
}
