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

    private bool initted = false;
    void Awake() 
    {
        abilityCharacter = GameObject.FindWithTag("Player").GetComponent<AbilityCharacter>();
        abilityTiles = GetComponentsInChildren<BaseTile>();
    }



    void Update() 
    {
        int i = 0;
        if (initted == false) {
            // 根据选好的技能, 通过id查询json得到sprite的key, 和ability类无关
            i = 0;
            foreach(var ability in abilityCharacter.Abilities) {
                if (abilityTiles.Length > i) {
                    Sprite sprite = Resources.Load<Sprite>(ability.Value.IconKey);
                    abilityTiles[i].SetSprite(sprite, Color.white);
                }
                i++;
            }
            initted = true;
        }

        i = 0;
        foreach(var ability in abilityCharacter.Abilities) {
            BaseTile tile = abilityTiles[i];
            CoolDownInfo info = GetCooldownOfAbility(ability.Value);
            float remainingPercent = info.isCooling ? 1 - info.elapsed / info.total : 0;
            tile.SetRemainingPercent(remainingPercent);
            i++;
        }
    }

    public CoolDownInfo GetCooldownOfAbility(Ability ability)
    {
        return ability.CalculateCooldown(abilityCharacter.AbilitySystem);
    }
}
