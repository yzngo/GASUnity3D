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

        // 仅用于描述GameplayEffect
        public GameplayEffectAddRemoveTagContainer AssetTags => assetTags;

        // 依赖于GameplayEffect的标签,同时也会赋予AbilitySystemComponent,
        // 移除时也会从ASC中移除, 只能用于Duration和Infinite的GameEffect
        public GameplayEffectAddRemoveTagContainer GrantedTags => grantedTags;

        // 决定Modifier是开还是关  Ongoing(持续存在的), 仅作用于Duration, Infinate
        public GameplayEffectRequireIgnoreTagContainer OngoingTagRequirements => ongoingTagRequirements;

        // 目标对象上有这些tag才可以应用
        public GameplayEffectRequireIgnoreTagContainer ApplicationTagRequirements => applicationTagRequirements;

        // 成功应用到Target上的话, 则移除Target身上的这些标签
        public GameplayEffectAddRemoveStacksTagContainer RemoveGameplayEffectsWithTag => removeGameplayEffectsWithTag;

    }

}
