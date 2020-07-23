using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem.Abilities;
using static GameplayAbilitySystem.Abilities.Ability;
using AbilitySystemDemo;

[DisallowMultipleComponent]
public class AbilityHotbar : MonoBehaviour 
{
    private AbilityCharacter abilityCharacter;
    private BaseTile[] abilityTiles;

    void Awake() 
    {
        abilityCharacter = GameObject.FindWithTag("Player").GetComponent<AbilityCharacter>();
        abilityTiles = GetComponentsInChildren<BaseTile>();

        for (int i = 0; i < abilityCharacter.abilities.Count; i++) {
            if (abilityTiles.Length > i) {
                Sprite sprite = Resources.Load<Sprite>(abilityCharacter.abilities[i].ability.IconKey);
                abilityTiles[i].SetSprite(sprite, Color.white);
            }
        }
    }

    void Update() 
    {
        for (int i = 0; i < abilityTiles.Length; i++) {
            BaseTile button = abilityTiles[i];
            CoolDownInfo info = GetCooldownOfAbility(i);
            float remainingPercent = info.isCooling ? 1 - info.elapsed / info.total : 0;
            button.SetRemainingPercent(remainingPercent);
        }
    }

    public CoolDownInfo GetCooldownOfAbility(int n)
    {
        if (n >= abilityCharacter.abilities.Count) {
            return new CoolDownInfo();
        }
        Ability ability = abilityCharacter.abilities[n].ability;
        CoolDownInfo cooldownInfo = ability.CalculateCooldown(abilityCharacter.AbilitySystem);
        return cooldownInfo;
    }
}
