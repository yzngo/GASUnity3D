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
    private AbilitySystem abilitySystem;

    void Start()
    {
        abilitySystem = GetComponent<AbilitySystem>();
    }

    public CoolDownInfo GetCooldownOfAbility(int n)
    {
        if (n >= Abilities.Count) {
            return new CoolDownInfo();
        }
        GameplayAbility ability = Abilities[n].ability;
        CoolDownInfo cooldownInfo = ability.CalculateCooldown(abilitySystem);
        return cooldownInfo;
    }

    public void CastAbility(int n)
    {
        if (n >= Abilities.Count) return;
        if (Abilities[n] == null) return;
        if (Abilities[n].ability == null) return;
        if (Abilities[n].target == null) return;
        var Ability = Abilities[n].ability;
        var Target = Abilities[n].target;
        var eventTag = Ability.Tags.AbilityTags.Added.Count > 0 ? Ability.Tags.AbilityTags.Added[0] : new GameplayTag();
        var eventData = new GameplayEventData();
        eventData.EventTag = eventTag;
        eventData.Target = Target;

        // If ability can be activated
        if (abilitySystem.TryActivateAbility(Ability))
        {
            // Send gameplay event to this player with information on target etc
            abilitySystem.OnGameplayEvent.Invoke(eventTag, eventData);
        }
    }

}
