using System;
using UnityEngine;
using System.Collections.Generic;
namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class EffectTagContainer
    {

        [Tooltip("Only for durational effect.????")]
        [SerializeField] private List<GameplayTag> grantedToInstigatorTags = default;

        [Tooltip("Effects with these Tags will removed when this effect execute.")]
        [SerializeField] private List<BeRemovedEffectInfo> removedEffectsTags = default;


        public List<GameplayTag> GrantedToInstigatorTags => grantedToInstigatorTags;
        public List<BeRemovedEffectInfo> RemovedEffectsId => removedEffectsTags;


        [Serializable]
        public class BeRemovedEffectInfo 
        {
            [Tooltip("GameplayEffects with this id will be candidates for removal")]
            public int Id;
            [Tooltip("Number of stacks of each GameEffect to remove.  0 means remove all stacks.")]
            public int BeRemovedStacks = 0;

        }





















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
