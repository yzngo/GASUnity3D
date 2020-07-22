using System;
using UnityEngine;

namespace GameplayAbilitySystem.Cues 
{
    [CreateAssetMenu(fileName = "GameplayCue", menuName = "Ability System/Cues/Cues")]
    public sealed class EffectCues : ScriptableObject 
    {
        [SerializeField] private BaseCueAction ExecuteAction = default;
        [SerializeField] private BaseCueAction OnActiveAction = default;
        [SerializeField] private BaseCueAction OnRemoveAction = default;

        public void HandleCue(AbilitySystem target, CueEventMomentType moment) {
            switch (moment) {
                case CueEventMomentType.OnActive:
                    OnActiveAction?.Execute(target);
                    break;
                case CueEventMomentType.OnExecute:
                    ExecuteAction?.Execute(target);
                    break;
                case CueEventMomentType.OnRemove:
                    OnRemoveAction?.Execute(target);
                    break;
            }
        }
    }

    public enum CueEventMomentType 
    {
        OnActive,       // Called when Cue is first activated
        OnExecute,      // Called when a Cue is executed (e.g. instant/periodic/tick)
        OnRemove        // Called when a Cue is removed
    }
}
