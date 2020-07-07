using GameplayAbilitySystem.Interfaces;
using UnityEngine;


namespace GameplayAbilitySystem.Attributes {

    // 继承此类实现OnAttributeChange函数, 定义如何修改attribute
    public abstract class BaseAttributeChangeHandler : ScriptableObject {

        public virtual void OnAttributeChange(IAttributeSet AttributeSet, IAttribute Attribute, ref float Value) {

        }
    }
}
