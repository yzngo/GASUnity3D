using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem;

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
            i = 0;
            foreach(var ability in abilityCharacter.Abilities) {
                if (abilityTiles.Length > i) {
                    KV.abilityId_SpriteKey.TryGetValue(ability.Value.Id, out var iconKey);
                    Sprite sprite = Resources.Load<Sprite>(iconKey);
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
