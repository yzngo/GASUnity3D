using System.Linq;
using UnityEngine;
using GameplayAbilitySystem;
using System.Collections.Generic;
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
        IEnumerable<(EffectContext EffectContext, int Stacks)> effectsInfo = instigator.GetAllDurationalEffects();
        if (effectsInfo == null) {
            return;
        }
        int tileIndex = 0;
        foreach(var effectInfo in effectsInfo) {
            if (effectTiles.Length < tileIndex ) return;
            if (effectInfo.EffectContext.Effect.Configs.DisplayWhenActived == false) {
                continue;
            }

            float elapsedTime = effectInfo.EffectContext.ElapsedDuration;
            float totalTime = effectInfo.EffectContext.TotalDuration;
            float remainingPercent =  totalTime > 0 ? 1 - elapsedTime / totalTime : 0;
            
            effectTiles[tileIndex].SetRemainingPercent(remainingPercent);
            
            KV.effectId_SpriteKey.TryGetValue(effectInfo.EffectContext.Effect.Configs.Id, out string iconKey);
            Sprite sprite = Resources.Load<Sprite>(iconKey);

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
