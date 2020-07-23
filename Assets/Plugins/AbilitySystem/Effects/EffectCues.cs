using System;
using UnityEngine;
using UnityEngine.Serialization;
namespace GameplayAbilitySystem.Effects 
{
    [CreateAssetMenu(fileName = "GameplayCue", menuName = "Ability System/Cues/Cues")]
    public sealed class EffectCues : ScriptableObject 
    {
        [SerializeField] private BaseCueAction OnActiveAction = default;
        [SerializeField] private BaseCueAction OnExecuteAction = default;
        [SerializeField] private BaseCueAction OnRemoveAction = default;

        public void Reset(BaseCueAction onActive, BaseCueAction onExecute, BaseCueAction onRemove)
        {
            OnActiveAction = onActive;
            OnExecuteAction = onExecute;
            OnRemoveAction = onRemove;
        }

        public void HandleCue(AbilitySystem target, CueEventMomentType moment) {
            switch (moment) {
                case CueEventMomentType.OnActive:
                    OnActiveAction?.Execute(target);
                    break;
                case CueEventMomentType.OnExecute:
                    OnExecuteAction?.Execute(target);
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
