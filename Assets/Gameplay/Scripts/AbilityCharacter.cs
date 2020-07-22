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

    void Awake()
    {
        AbilitySystem = GetComponent<AbilitySystem>();
        var temp = new List<CastingAbilityContainer>() {
            new CastingAbilityContainer() {
                ability = TestData.CreateAbility("fire"),
                target = null,
            },
            new CastingAbilityContainer() {
                ability = TestData.CreateAbility("bloodPact"),
                target = null,
            },
            new CastingAbilityContainer() {
                ability = TestData.CreateAbility("heal"),
                target = null,
            },
        };
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
