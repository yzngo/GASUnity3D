using System.Collections.Generic;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Abilities;
using UnityEngine;
using System;
using UnityEngine.Serialization;
public class EnemyCharacter : MonoBehaviour
{
    public AbilitySystem AbilitySystem { get; private set; }

    void Awake()
    {
        AbilitySystem = GetComponent<AbilitySystem>();
    }
}
