using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem.Effects;
using GameplayAbilitySystem;

public class EffectsBar : MonoBehaviour {
    private AbilitySystem instigator;
    private GameplayTagStatusBarButton[] GameplayTagIndicator;

    private void Awake() {
        instigator = GameObject.FindWithTag("Player").GetComponent<AbilitySystem>();
        GameplayTagIndicator = GetComponentsInChildren<GameplayTagStatusBarButton>();
    }

    void Update() 
    {
        var effectsInfo = instigator.GetDurationEffects()
                    .OrderBy(x => x.StartTime)
                    .GroupBy(x => x.Effect.Id)
                    .Select(x => (EffectContext: x.Last(), Stacks: x.Count()));

        int tileIndex = 0;
        foreach(var effectInfo in effectsInfo) {
            if (GameplayTagIndicator.Length < tileIndex ) return;
            if (effectInfo.EffectContext.Effect.Icon == null) continue;

            float elapsedTime = effectInfo.EffectContext.ElapsedTime;
            float totalTime = effectInfo.EffectContext.TotalTime;
            float remainingPercent =  totalTime > 0 ? 1 - elapsedTime / totalTime : 0;

            GameplayTagIndicator[tileIndex].SetRemainingPercent(remainingPercent);
            GameplayTagIndicator[tileIndex].ImageIcon.sprite = effectInfo.EffectContext.Effect.Icon;
            GameplayTagIndicator[tileIndex].ImageIcon.color = new Color( 1.0f, 1.0f, 1.0f);
            GameplayTagIndicator[tileIndex].GetComponentInChildren<RectTransform>(true).gameObject.SetActive(true);
            GameplayTagIndicator[tileIndex].SetStacks(effectInfo.Stacks);
            tileIndex++;
        }
        for (int i = tileIndex; i < GameplayTagIndicator.Length; i++) {
            GameplayTagIndicator[i].ImageIcon.sprite = null;
            GameplayTagIndicator[i].ImageIcon.color = new Color(0, 0, 0, 0);
            GameplayTagIndicator[i].SetRemainingPercent(0);
            GameplayTagIndicator[i].GetComponentInChildren<RectTransform>(true).gameObject.SetActive(false);
            GameplayTagIndicator[i].SetStacks(0);
        }
    }
}
