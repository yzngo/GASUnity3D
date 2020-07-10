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
        public Ability ability;
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

        Ability ability = Abilities[n].ability;
        AbilitySystem target = Abilities[n].target;
        AbilitySystem.TryActivateAbility(ability, target);
    }
}
