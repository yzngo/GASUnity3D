using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem.Effects;
using GameplayAbilitySystem;

public class EffectsBar : MonoBehaviour {
    public AbilitySystem instigator;
    public List<GameplayTagStatusBarButton> GameplayTagIndicator;
    public List<GameplayTagIconMap> GameplayTagIcons;

    private Dictionary<GameplayTag, GameplayTagIconMap> availableTagsToShow;

    void Awake() 
    {
        availableTagsToShow = GameplayTagIcons.ToDictionary(x => x.Tag);
    }

    List<(GameplayTag Tag, EffectContext effectContext, int stacks)> GetTagsToShow() 
    {
        return instigator.GetActiveEffectsTags()
                            .Where(x => availableTagsToShow
                                                    .ContainsKey(x.Tag))
                                                    .OrderBy(x => x.GrantingEffect.StartTime)
                                                    .Select(x => (x.Tag, x.GrantingEffect, 1))
                            .ToList();
    }


    void Update() 
    {
        // var stackedEffectsToShow = GetEffectsToShow();
        // var stackedTagsToShow = GetStackedGameplayTagsToShow();
        var temp = instigator.GetActiveEffectsTags()
                            .Where(x => availableTagsToShow
                                                    .ContainsKey(x.Tag))
                                                    .OrderBy(x => x.GrantingEffect.StartTime)
                                                    .Select(x => (x.Tag, x.GrantingEffect, 1))
                            .ToList();

        var stackedTagsToShow = GetTagsToShow().GroupBy(x => x.Tag)
                            .Select(x => (x.Last().Tag, x.Last().effectContext, x.Count()))
                            .ToList();




        var tagIndex = 0;
        for (int i = 0; i < stackedTagsToShow.Count; i++) {
            var tagToShow = stackedTagsToShow[i];

            var stacks = stackedTagsToShow[i].Item3;
            // No more space to show buffs - just ignore the rest until space opens up
            if (GameplayTagIndicator.Count < tagIndex) return;

            availableTagsToShow.TryGetValue(tagToShow.Tag, out var iconMap);
            if (iconMap == null) continue;

            var cooldownElapsed = tagToShow.effectContext.CooldownTimeElapsed;
            var cooldownTotal = tagToShow.effectContext.CooldownTimeTotal;

            var remainingPercent = 0f;
            if (cooldownTotal != 0) {
                remainingPercent = 1 - cooldownElapsed / cooldownTotal;
            }

            GameplayTagIndicator[tagIndex].SetCooldownRemainingPercent(1 - remainingPercent);
            GameplayTagIndicator[tagIndex].ImageIcon.sprite = iconMap.Sprite;
            GameplayTagIndicator[tagIndex].ImageIcon.color = new Color( 1.0f, 1.0f, 1.0f);
            GameplayTagIndicator[tagIndex].GetComponentInChildren<RectTransform>(true).gameObject.SetActive(true);
            GameplayTagIndicator[tagIndex].SetStacks(stacks);
            ++tagIndex;
        }

        // These are all empty - reset them
        for (int i = tagIndex; i < GameplayTagIndicator.Count; i++) {
            GameplayTagIndicator[i].ImageIcon.sprite = null;
            GameplayTagIndicator[i].ImageIcon.color = new Color(0, 0, 0, 0);
            GameplayTagIndicator[i].SetCooldownRemainingPercent(0);
            GameplayTagIndicator[i].GetComponentInChildren<RectTransform>(true).gameObject.SetActive(false);
            GameplayTagIndicator[i].SetStacks(0);
        }
    }
}
