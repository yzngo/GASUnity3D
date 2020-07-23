using System.Linq;
using UnityEngine;
using GameplayAbilitySystem;
using System.Collections.Generic;
using GameplayAbilitySystem.Effects;
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
        IEnumerable<(EffectContext EffectContext, int Stacks)> effectsInfo = 
                instigator.GetAllDurationalEffects()
                    .OrderBy(x => x.StartTime)
                    .GroupBy(x => x.Effect.Configs.Id)
                    .Select(x => ( x.First(), x.Count()));
        int tileIndex = 0;
        foreach(var effectInfo in effectsInfo) {
            if (effectTiles.Length < tileIndex ) return;
            if (effectInfo.EffectContext.Effect.Configs.IconKey.Length <= 0) {
                continue;
            }

            float elapsedTime = effectInfo.EffectContext.ElapsedDuration;
            float totalTime = effectInfo.EffectContext.TotalDuration;
            float remainingPercent =  totalTime > 0 ? 1 - elapsedTime / totalTime : 0;
            

            effectTiles[tileIndex].SetRemainingPercent(remainingPercent);
            Sprite sprite = Resources.Load<Sprite>(effectInfo.EffectContext.Effect.Configs.IconKey);
            effectTiles[tileIndex].SetSprite(sprite,  Color.white);
            effectTiles[tileIndex].GetComponentInChildren<RectTransform>(true).gameObject.SetActive(true);
            effectTiles[tileIndex].SetStacks(effectInfo.Stacks);
            tileIndex++;
        }

        for (int i = tileIndex; i < effectTiles.Length; i++) {
            effectTiles[i].SetSprite(null, Color.clear);
            effectTiles[i].SetRemainingPercent(0);
            effectTiles[i].GetComponentInChildren<RectTransform>(true).gameObject.SetActive(false);
            effectTiles[i].SetStacks(0);
        }
    }
}
