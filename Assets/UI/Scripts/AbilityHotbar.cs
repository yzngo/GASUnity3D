using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem.Abilities;
using static GameplayAbilitySystem.Abilities.Ability;

public class AbilityHotbar : MonoBehaviour {
    public AbilityCharacter AbilityCharacter;
    public List<GenericUIIcon> AbilityButtons;

    void Awake() {
        for (int i = 0; i < AbilityCharacter.abilities.Count; i++) {
            if (AbilityButtons.Count > i) {
                AbilityButtons[i].ImageIcon.sprite = AbilityCharacter.abilities[i].ability.Icon;
                AbilityButtons[i].ImageIcon.color = new Color(1.0f, 1.0f, 1.0f); 
            }
        }

    }

    void Update() {
        for (int i = 0; i < AbilityButtons.Count; i++) {
            var button = AbilityButtons[i];
            CoolDownInfo info = GetCooldownOfAbility(i);
            var remainingPercent = 0f;
            if (info.isCooling == true) {
                remainingPercent = 1 - info.elapsed / info.total;
            }
            button.SetCooldownRemainingPercent(1 - remainingPercent);
        }

    }

    public CoolDownInfo GetCooldownOfAbility(int n)
    {
        if (n >= AbilityCharacter.abilities.Count) {
            return new CoolDownInfo();
        }
        Ability ability = AbilityCharacter.abilities[n].ability;
        CoolDownInfo cooldownInfo = ability.CalculateCooldown(AbilityCharacter.AbilitySystem);
        return cooldownInfo;
    }
}
