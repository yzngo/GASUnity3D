using System.Collections.Generic;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Abilities;
using UnityEngine;
using System;

public class AbilityCharacter : MonoBehaviour
{
    public AbilitySystem AbilitySystem { get; private set; }

    public List<CastingAbilityContainer> Abilities = new List<CastingAbilityContainer>();

    void Start()
    {
        AbilitySystem = GetComponent<AbilitySystem>();
    }

    public (float CooldownElapsed, float CooldownTotal) GetCooldownOfAbility(int n)
    {
        if (n >= this.Abilities.Count) return (0f, 0f);
        var ability = this.Abilities[n].Ability;
        return ability.CalculateCooldown(AbilitySystem);
        // foreach (var item in SelfAbilitySystem.ActiveGameplayEffectsContainer.ActiveCooldowns)
        // {
        //     Debug.Log(item.Effect.GameplayEffectPolicy.DurationMagnitude - item.CooldownTimeElapsed);
        // }
    }

    public void CastAbility(int n)
    {
        if (n >= this.Abilities.Count) return;
        if (this.Abilities[n] == null) return;
        if (this.Abilities[n].Ability == null) return;
        if (this.Abilities[n].AbilityTarget == null) return;
        var Ability = this.Abilities[n].Ability;
        var Target = this.Abilities[n].AbilityTarget;
        var eventTag = Ability.Tags.AbilityTags.Added.Count > 0 ? Ability.Tags.AbilityTags.Added[0] : new GameplayTag();
        var eventData = new GameplayEventData();
        eventData.EventTag = eventTag;
        eventData.Target = Target;

        // If ability can be activated
        if (AbilitySystem.TryActivateAbility(Ability))
        {
            // Send gameplay event to this player with information on target etc
            AbilitySystem.OnGameplayEvent.Invoke(eventTag, eventData);
        }
    }

}

[Serializable]
public class CastingAbilityContainer
{
    public GameplayAbility Ability;

    public AbilitySystem AbilityTarget;
}