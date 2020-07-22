using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Abilities;
public static class TestData
{
    public static Ability CreateAbility(string abilityId)
    {
        var ability = ScriptableObject.CreateInstance("Ability") as Ability;
        return ability;
    }
}
