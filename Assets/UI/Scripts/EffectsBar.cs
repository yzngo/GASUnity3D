using System.Linq;
using UnityEngine;
using GameplayAbilitySystem;

public class EffectsBar : MonoBehaviour 
{
    private AbilitySystem instigator;
    private EffectTile[] effectTiles;

    private void Awake() 
    {
        instigator = GameObject.FindWithTag("Player").GetComponent<AbilitySystem>();
        effectTiles = GetComponentsInChildren<EffectTile>();
    }

    void Update() 
    {
        var effectsInfo = instigator.GetDurationEffects()
                    .OrderBy(x => x.StartTime)
                    .GroupBy(x => x.Effect.Id)
                    .Select(x => (EffectContext: x.Last(), Stacks: x.Count()));

        int tileIndex = 0;
        foreach(var effectInfo in effectsInfo) {
            if (effectTiles.Length < tileIndex ) return;
            if (effectInfo.EffectContext.Effect.Icon == null) continue;

            float elapsedTime = effectInfo.EffectContext.ElapsedTime;
            float totalTime = effectInfo.EffectContext.TotalTime;
            float remainingPercent =  totalTime > 0 ? 1 - elapsedTime / totalTime : 0;

            effectTiles[tileIndex].SetRemainingPercent(remainingPercent);
            effectTiles[tileIndex].ImageIcon.sprite = effectInfo.EffectContext.Effect.Icon;
            effectTiles[tileIndex].ImageIcon.color = new Color( 1.0f, 1.0f, 1.0f);
            effectTiles[tileIndex].GetComponentInChildren<RectTransform>(true).gameObject.SetActive(true);
            effectTiles[tileIndex].SetStacks(effectInfo.Stacks);
            tileIndex++;
        }

        for (int i = tileIndex; i < effectTiles.Length; i++) {
            effectTiles[i].ImageIcon.sprite = null;
            effectTiles[i].ImageIcon.color = new Color(0, 0, 0, 0);
            effectTiles[i].SetRemainingPercent(0);
            effectTiles[i].GetComponentInChildren<RectTransform>(true).gameObject.SetActive(false);
            effectTiles[i].SetStacks(0);
        }
    }
}
