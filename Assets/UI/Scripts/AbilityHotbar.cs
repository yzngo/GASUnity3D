using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem.Abilities;
using static GameplayAbilitySystem.Abilities.Ability;

public class AbilityHotbar : MonoBehaviour 
{
    private AbilityCharacter abilityCharacter;
    public List<BaseTile> AbilityButtons;

    void Awake() 
    {
        abilityCharacter = GameObject.FindWithTag("Player").GetComponent<AbilityCharacter>();
        for (int i = 0; i < abilityCharacter.abilities.Count; i++) {
            if (AbilityButtons.Count > i) {
                AbilityButtons[i].ImageIcon.sprite = abilityCharacter.abilities[i].ability.Icon;
                AbilityButtons[i].ImageIcon.color = new Color(1.0f, 1.0f, 1.0f); 
            }
        }
    }

    void Update() 
    {
        for (int i = 0; i < AbilityButtons.Count; i++) {
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
