using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Effects;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities 
{
    [Serializable]
    public class GameplayAbilityTags
    {
        public GameplayEffectAddRemoveTagContainer AbilityTags => _abilityTags;
        public GameplayEffectAddRemoveTagContainer CooldownTags => _cooldownTags;
        
        // public GameplayEffectAddRemoveTagContainer CancelAbilitiesWithTags => _cancelAbilitiesWithTags;
        
        // public GameplayEffectAddRemoveTagContainer BlockAbilitiesWithTags => _blockAbilitiesWithTags;
        
        // public GameplayEffectAddRemoveTagContainer ActivationOwnedTags => _activationOwnedTags;
        
        // public GameplayEffectAddRemoveTagContainer ActivationRequiredTags => _activationRequiredTags;
        
        // public GameplayEffectAddRemoveTagContainer ActivationBlockedTags => _activationBlockedTags;
        
        // public GameplayEffectAddRemoveTagContainer SourceRequiredTags => _sourceRequiredTags;
        
        // public GameplayEffectAddRemoveTagContainer SourceBlockedTags => _sourceBlockedTags;
        
        // public GameplayEffectAddRemoveTagContainer TargetRequiredTags => _targetRequiredTags;
        
        // public GameplayEffectAddRemoveTagContainer TargetBlockedTags => _targetBlockedTags;


        [Tooltip("Tags for this ability")]
        [SerializeField] protected GameplayEffectAddRemoveTagContainer _abilityTags;

        [Tooltip("Tags to determine whether the ability is on cooldown")]
        [SerializeField] protected GameplayEffectAddRemoveTagContainer _cooldownTags;

        // [Tooltip("Active abilities on player with this AbilitySystem which have these tags are cancelled")]
        // [SerializeField] protected GameplayEffectAddRemoveTagContainer _cancelAbilitiesWithTags;

        // [Tooltip("Abilities with these tags will be blocked")]
        // [SerializeField] protected GameplayEffectAddRemoveTagContainer _blockAbilitiesWithTags;

        // [Tooltip("Tags to apply to activating owner while this ability is active")]
        // [SerializeField] protected GameplayEffectAddRemoveTagContainer _activationOwnedTags;

        // [Tooltip("Ability can only be activated if the activating object has all of these tags")]
        // [SerializeField] protected GameplayEffectAddRemoveTagContainer _activationRequiredTags;

        // [Tooltip("Ability is blocked if activating object has any of these tags")]
        // [SerializeField] protected GameplayEffectAddRemoveTagContainer _activationBlockedTags;

        // [Tooltip("Ability can only be activated if source object has all of these tags")]
        // [SerializeField] protected GameplayEffectAddRemoveTagContainer _sourceRequiredTags;

        // [Tooltip("Ability is blocked if source object has any of these tags")]
        // [SerializeField] protected GameplayEffectAddRemoveTagContainer _sourceBlockedTags;

        // [Tooltip("Ability can only be activated if source object has all of these tags")]
        // [SerializeField] protected GameplayEffectAddRemoveTagContainer _targetRequiredTags;

        // [Tooltip("Ability is blocked if source object has any of these tags")]
        // [SerializeField] protected GameplayEffectAddRemoveTagContainer _targetBlockedTags;
        
    }
}
