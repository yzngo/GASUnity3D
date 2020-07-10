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
        [SerializeField] private List<GameplayTag> effectTags = default;

        [Tooltip("Only for durational effect.????")]
        [SerializeField] private List<GameplayTag> grantedToInstigatorTags = default;

        [Tooltip("Effects with these Tags will removed when this effect execute.")]
        [SerializeField] private List<BeRemovedEffectInfo> removedEffectsTags = default;


        public List<GameplayTag> EffectTags => effectTags;
        public List<GameplayTag> GrantedToInstigatorTags => grantedToInstigatorTags;
        public List<BeRemovedEffectInfo> RemovedEffectsTags => removedEffectsTags;
























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
