using System.Collections.Generic;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Abilities;
using UnityEngine;
using System;

public class AbilityCharacter : MonoBehaviour
{
    [Serializable]
    public class CastingAbilityContainer
    {
        public GameplayAbility ability;
        public AbilitySystem target;
    }

    public List<CastingAbilityContainer> Abilities = new List<CastingAbilityContainer>();
    public AbilitySystem AbilitySystem { get; private set; }

    void Start()
    {
        AbilitySystem = GetComponent<AbilitySystem>();
    }

    public void CastAbility(int n)
    {
        if (n >= Abilities.Count) return;
        if (Abilities[n] == null) return;
        if (Abilities[n].ability == null) return;
        if (Abilities[n].target == null) return;

        GameplayAbility ability = Abilities[n].ability;
        AbilitySystem target = Abilities[n].target;
        GameplayTag abilityTag = ability.Tags.AbilityTags.Added.Count > 0 ? ability.Tags.AbilityTags.Added[0] : new GameplayTag();

        AbilityEventData abilityEventData = new AbilityEventData();
        // eventData.EventTag = eventTag;
        abilityEventData.Target = target;

        // If ability can be activated
        if (AbilitySystem.TryActivateAbility(ability))
        {
            // Send gameplay event to this player with information on target etc
            AbilitySystem.OnGameplayEvent.Invoke(abilityTag, abilityEventData);
        }
    }

}
