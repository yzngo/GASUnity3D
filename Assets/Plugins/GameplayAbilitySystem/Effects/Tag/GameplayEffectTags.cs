using System;
using GAS.Interfaces;
using UnityEngine;

namespace GAS.GameplayEffects {

    [Serializable]
    public class GameplayEffectTags : IGameplayEffectTags {
        [SerializeField]
        private GameplayEffectAddRemoveTagContainer effectTags = new GameplayEffectAddRemoveTagContainer();

        [SerializeField]
        private GameplayEffectAddRemoveTagContainer grantedToASCTags = new GameplayEffectAddRemoveTagContainer();

        [SerializeField]
        private GameplayEffectRequireIgnoreTagContainer ongoingRequiredTags = new GameplayEffectRequireIgnoreTagContainer();

        [SerializeField]
        private GameplayEffectRequireIgnoreTagContainer applyRequiredTags = new GameplayEffectRequireIgnoreTagContainer();

        [SerializeField]
        private GameplayEffectAddRemoveStacksTagContainer beRemovedEffectsTags = new GameplayEffectAddRemoveStacksTagContainer();

        // 此Effect具有的标签
        public GameplayEffectAddRemoveTagContainer EffectTags => effectTags;

        // 赋予AbilitySystemComponent的标签
        // 移除时也会从ASC中移除, 只能用于Duration和Infinite的GameEffect
        public GameplayEffectAddRemoveTagContainer GrantedToASCTags => grantedToASCTags;
        // ongoing (持续存在的)
        // 决定Modifier是开还是关  Ongoing(持续存在的), 仅作用于Duration, Infinate
        public GameplayEffectRequireIgnoreTagContainer OngoingRequiredTags => ongoingRequiredTags;

        // 目标对象上有这些tag才可以应用
        public GameplayEffectRequireIgnoreTagContainer ApplyRequiredTags => applyRequiredTags;

        // 移除任何带有这些tag的effect
        public GameplayEffectAddRemoveStacksTagContainer BeRemovedEffectsTags => beRemovedEffectsTags;

    }

}
