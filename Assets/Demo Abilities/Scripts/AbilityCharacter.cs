using System.Collections.Generic;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Abilities;
using UnityEngine;
using System;
using UnityEngine.Serialization;
public class AbilityCharacter : MonoBehaviour
{
    [Serializable]
    public class CastingAbilityContainer
    {
        public Ability ability;
        public AbilitySystem target;
    }

    [FormerlySerializedAs("Abilities")]
    public List<CastingAbilityContainer> abilities = new List<CastingAbilityContainer>();
    public AbilitySystem AbilitySystem { get; private set; }

    void Start()
    {
        AbilitySystem = GetComponent<AbilitySystem>();
    }

    public void CastAbility(int n)
    {
        if (n >= abilities.Count) return;
        if (abilities[n] == null) return;
        if (abilities[n].ability == null) return;
        if (abilities[n].target == null) return;

        Ability ability = abilities[n].ability;
        AbilitySystem target = abilities[n].target;
        AbilitySystem.TryActivateAbility(ability, target);
    }
}
