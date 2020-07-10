using UnityEngine;


namespace GameplayAbilitySystem.Attributes 
{
    // 继承此类实现OnAttributeChange函数, 定义如何修改attribute
    public abstract class AttributeChangeHandler : ScriptableObject 
    {
        public virtual void OnAttributeChange(AttributeSet attributeSet, Attribute attribute, ref float value) {
        }
    }
}
