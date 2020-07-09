using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem.Abilities;
public class AbilityHotbarManager : MonoBehaviour {
    public AbilityCharacter AbilityCharacter;
    public List<AbilityHotbarButton> AbilityButtons;
    public List<AbilityIconMap> AbilityIconMaps;

    void Awake() {
        for (int i = 0; i < AbilityCharacter.Abilities.Count; i++) {
            if (AbilityButtons.Count > i) {
                var abilityGraphic = AbilityIconMaps.FirstOrDefault(x => x.Ability == AbilityCharacter.Abilities[i].ability);

                if (abilityGraphic != null) {
                    AbilityButtons[i].ImageIcon.sprite = abilityGraphic.Sprite;
                    AbilityButtons[i].ImageIcon.color = abilityGraphic.SpriteColor;
                }
            }
        }

    }

    void Update() {
        for (int i = 0; i < AbilityButtons.Count; i++) {
            var button = AbilityButtons[i];
            CoolDownInfo info = AbilityCharacter.GetCooldownOfAbility(i);
            var remainingPercent = 0f;
            if (info.total != 0) {
                remainingPercent = 1 - info.elapsed / info.total;
            }
            button.SetCooldownRemainingPercent(1 - remainingPercent);
        }

    }

}
