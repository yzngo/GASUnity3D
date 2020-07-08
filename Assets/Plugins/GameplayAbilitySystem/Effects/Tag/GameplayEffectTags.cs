using System;
using GAS.Interfaces;
using UnityEngine;

namespace GAS.GameplayEffects {

    [Serializable]
    public class GameplayEffectTags : IGameplayEffectTags {
        [SerializeField]
        private GameplayEffectAddRemoveTagContainer assetTags = new GameplayEffectAddRemoveTagContainer();

        [SerializeField]
        private GameplayEffectAddRemoveTagContainer grantedTags = new GameplayEffectAddRemoveTagContainer();

        [SerializeField]
        private GameplayEffectRequireIgnoreTagContainer ongoingTagRequirements = new GameplayEffectRequireIgnoreTagContainer();

        [SerializeField]
        private GameplayEffectRequireIgnoreTagContainer applicationTagRequirements = new GameplayEffectRequireIgnoreTagContainer();

        [SerializeField]
        private GameplayEffectAddRemoveStacksTagContainer removeGameplayEffectsWithTag = new GameplayEffectAddRemoveStacksTagContainer();

        // 此Effect具有的标签
        public GameplayEffectAddRemoveTagContainer EffectTags => assetTags;

        // 赋予AbilitySystemComponent的标签
        // 移除时也会从ASC中移除, 只能用于Duration和Infinite的GameEffect
        public GameplayEffectAddRemoveTagContainer GrantedToASCTags => grantedTags;
        // ongoing (持续存在的)
        // 决定Modifier是开还是关  Ongoing(持续存在的), 仅作用于Duration, Infinate
        public GameplayEffectRequireIgnoreTagContainer OngoingRequiredTags => ongoingTagRequirements;

        // 目标对象上有这些tag才可以应用
        public GameplayEffectRequireIgnoreTagContainer ApplyRequiredTags => applicationTagRequirements;

        // 移除任何带有这些tag的effect
        public GameplayEffectAddRemoveStacksTagContainer BeRemovedEffectsTags => removeGameplayEffectsWithTag;

    }

}
