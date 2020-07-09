using System;
using UnityEngine;

namespace GAS.GameplayCues {
    [CreateAssetMenu(fileName = "GameplayCue", menuName = "Ability System/Gameplay Cue/Gameplay Cue")]
    public class GameplayCue : ScriptableObject {

        [SerializeField] private BaseGameplayCueAction ExecuteAction;

        [SerializeField] private BaseGameplayCueAction OnActiveAction;

        [SerializeField] private BaseGameplayCueAction WhileActiveAction;

        [SerializeField] private BaseGameplayCueAction OnRemoveAction;

        public void HandleGameplayCue(GameObject Target, EGameplayCueEvent Event) {
            switch (Event) {
                case EGameplayCueEvent.OnExecute:
                    if (ExecuteAction == null) break;
                    ExecuteAction.Action(Target);
                    break;
                case EGameplayCueEvent.OnActive:
                    if (OnActiveAction == null) break;
                    OnActiveAction.Action(Target);
                    break;
                case EGameplayCueEvent.WhileActive:
                    if (WhileActiveAction == null) break;
                    WhileActiveAction.Action(Target);
                    break;
                case EGameplayCueEvent.OnRemove:
                    if (OnRemoveAction == null) break;
                    OnRemoveAction.Action(Target);
                    break;
            }
        }
    }

    /// ?????
    /// <para>WhileActive/OnActive is called for Infinite effects</para>
    /// <para>Executed is called for Instant effects/on each tick</para>
    /// <para>WhileActive/OnActive/Removed is called for Duration effects</para>
    public enum EGameplayCueEvent {
        OnExecute,  // Called when a GameplayCue is executed (e.g. instant/periodic/tick)
        OnActive, // Called when GameplayCue is first activated
        WhileActive, // Called *while* GameplayCue is active
        OnRemove    // Called when a GameplayCue is removed
    }
}
