using GameplayAbilitySystem.Attributes;
using UnityEngine;

namespace AbilitySystemDemo
{
    public class StatChangeHandlerComponent : MonoBehaviour
    {
        public AbstractStatChangeHandler StatChangeHandler;

        public void Execute(Attribute attribute)
        {
            StatChangeHandler.StatChanged(attribute);
        }
    }
}


