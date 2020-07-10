using System;
using UnityEngine;

namespace GameplayAbilitySystem.Cues 
{
    [CreateAssetMenu(fileName = "GameplayCue", menuName = "Ability System/Gameplay Cue/Gameplay Cue")]
    public sealed class EffectCues : ScriptableObject 
    {
        [SerializeField] private BaseCueAction ExecuteAction = default;
        [SerializeField] private BaseCueAction OnActiveAction = default;
        [SerializeField] private BaseCueAction WhileActiveAction = default;
        [SerializeField] private BaseCueAction OnRemoveAction = default;

        public void HandleCue(AbilitySystem target, CueEventMomentType moment) {
            switch (moment) {
                case CueEventMomentType.OnActive:
                    if (OnActiveAction == null) break;
                    OnActiveAction.Action(target);
                    break;
                case CueEventMomentType.OnExecute:
                    if (ExecuteAction == null) break;
                    ExecuteAction.Action(target);
                    break;
                case CueEventMomentType.WhileActive:
                    if (WhileActiveAction == null) break;
                    WhileActiveAction.Action(target);
                    break;
                case CueEventMomentType.OnRemove:
                    if (OnRemoveAction == null) break;
                    OnRemoveAction.Action(target);
                    break;
            }
        }
    }

    /// ?????
    /// WhileActive/OnActive is called for Infinite effects
    /// OnExecute is called for Instant effects/on each tick
    /// WhileActive/OnActive/OnRemove is called for Duration effects
    public enum CueEventMomentType 
    {
        OnActive,       // Called when Cue is first activated
        OnExecute,      // Called when a Cue is executed (e.g. instant/periodic/tick)
        WhileActive,    // Called *while* Cue is active
        OnRemove        // Called when a Cue is removed
    }
}
