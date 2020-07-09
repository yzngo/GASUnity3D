using AbilitySystem.Interfaces;
using UnityEngine;


namespace AbilitySystem.Attributes {

    // 继承此类实现OnAttributeChange函数, 定义如何修改attribute
    public abstract class BaseAttributeChangeHandler : ScriptableObject {

        public virtual void OnAttributeChange(IAttributeSet attributeSet, IAttribute attribute, ref float value) {

        }
    }
}
