
using System;
using GameplayAbilitySystem.Abilities;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AbilityIconMap", menuName = "Ability System Demo/Ability Icon Map")]
public class AbilityIconMap : ScriptableObject {
    public Ability Ability;
    public Sprite Sprite;
    public Color SpriteColor;
}
