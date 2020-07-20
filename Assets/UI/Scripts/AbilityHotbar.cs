using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem.Abilities;
using static GameplayAbilitySystem.Abilities.Ability;

[DisallowMultipleComponent]
public class AbilityHotbar : MonoBehaviour 
{
    private AbilityCharacter abilityCharacter;
    private BaseTile[] AbilityButtons;

    void Awake() 
    {
        abilityCharacter = GameObject.FindWithTag("Player").GetComponent<AbilityCharacter>();
        AbilityButtons = GetComponentsInChildren<BaseTile>();

        for (int i = 0; i < abilityCharacter.abilities.Count; i++) {
            if (AbilityButtons.Length > i) {
                AbilityButtons[i].SetSprite(abilityCharacter.abilities[i].ability.Icon, Color.white);
            }
        }
    }

    void Update() 
    {
        for (int i = 0; i < AbilityButtons.Length; i++) {
            var button = AbilityButtons[i];
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
