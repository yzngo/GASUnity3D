using System;
using UnityEngine;

namespace AbilitySystem.Cues {
    [CreateAssetMenu(fileName = "GameplayCue", menuName = "Ability System/Gameplay Cue/Gameplay Cue")]
    public class GameplayCue : ScriptableObject {

        [SerializeField] private BaseCueAction ExecuteAction = default;

        [SerializeField] private BaseCueAction OnActiveAction = default;

        [SerializeField] private BaseCueAction WhileActiveAction = default;

        [SerializeField] private BaseCueAction OnRemoveAction = default;

        public void HandleCue(GameObject target, CueEventMoment moment) {
            switch (moment) {
                case CueEventMoment.OnExecute:
                    if (ExecuteAction == null) break;
                    ExecuteAction.Action(target);
                    break;
                case CueEventMoment.OnActive:
                    if (OnActiveAction == null) break;
                    OnActiveAction.Action(target);
                    break;
                case CueEventMoment.WhileActive:
                    if (WhileActiveAction == null) break;
                    WhileActiveAction.Action(target);
                    break;
                case CueEventMoment.OnRemove:
                    if (OnRemoveAction == null) break;
                    OnRemoveAction.Action(target);
                    break;
            }
        }
    }

    /// ?????
    /// <para>WhileActive/OnActive is called for Infinite effects</para>
    /// <para>Executed is called for Instant effects/on each tick</para>
    /// <para>WhileActive/OnActive/Removed is called for Duration effects</para>
    public enum CueEventMoment {
        OnExecute,  // Called when a GameplayCue is executed (e.g. instant/periodic/tick)
        OnActive, // Called when GameplayCue is first activated
        WhileActive, // Called *while* GameplayCue is active
        OnRemove    // Called when a GameplayCue is removed
    }
}
