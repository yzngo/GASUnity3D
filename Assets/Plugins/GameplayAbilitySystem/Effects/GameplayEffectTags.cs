using System;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayEffects {
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

        public GameplayEffectAddRemoveTagContainer AssetTags => assetTags;
        public GameplayEffectAddRemoveTagContainer GrantedTags => grantedTags;
        public GameplayEffectRequireIgnoreTagContainer OngoingTagRequirements => ongoingTagRequirements;
        public GameplayEffectRequireIgnoreTagContainer ApplicationTagRequirements => applicationTagRequirements;
        public GameplayEffectAddRemoveStacksTagContainer RemoveGameplayEffectsWithTag => removeGameplayEffectsWithTag;

    }

}
