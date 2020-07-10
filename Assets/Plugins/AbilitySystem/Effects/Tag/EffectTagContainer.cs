using System;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;
using System.Collections.Generic;
namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class EffectTagContainer
    {
        [Tooltip("Tags for this effect.")]
        [SerializeField] private List<GameplayTag> effectTags = new List<GameplayTag>();

        [Tooltip("Only for durational effect.")]
        [SerializeField] private List<GameplayTag> grantedToInstigatorTags = new List<GameplayTag>();

        [Tooltip("Effects with these Tags will removed when this effect execute.")]
        [SerializeField] private RemovedEffectTagContainer removedEffectsTags = new RemovedEffectTagContainer();


        public List<GameplayTag> EffectTags => effectTags;
        // 赋予AbilitySystemComponent的标签
        // 移除时也会从ASC中移除, 只能用于Duration和Infinite的GameEffect
        public List<GameplayTag> GrantedToInstigatorTags => grantedToInstigatorTags;
        public RemovedEffectTagContainer RemovedEffectsTags => removedEffectsTags;

        // [SerializeField]
        // private GameplayEffectRequireIgnoreTagContainer ongoingRequiredTags = new GameplayEffectRequireIgnoreTagContainer();
        // [SerializeField]
        // private GameplayEffectRequireIgnoreTagContainer applyRequiredTags = new GameplayEffectRequireIgnoreTagContainer();
        // ongoing (持续存在的)
        // 决定Modifier是开还是关  Ongoing(持续存在的), 仅作用于Duration, Infinate
        // public GameplayEffectRequireIgnoreTagContainer OngoingRequiredTags => ongoingRequiredTags;
        // 目标对象上有这些tag才可以应用
        // public GameplayEffectRequireIgnoreTagContainer ApplyRequiredTags => applyRequiredTags;
    }
}
