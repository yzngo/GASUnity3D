
using System;
using AbilitySystem.Abilities;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AbilityIconMap", menuName = "Ability System Demo/Ability Icon Map")]
public class AbilityIconMap : ScriptableObject {
    public GameplayAbility Ability;
    public Sprite Sprite;
    public Color SpriteColor;
}
