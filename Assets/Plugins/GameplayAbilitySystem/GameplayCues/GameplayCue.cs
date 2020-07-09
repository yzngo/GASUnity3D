using System;
using UnityEngine;

namespace GAS.GameplayCues {
    [CreateAssetMenu(fileName = "GameplayCue", menuName = "Ability System/Gameplay Cue/Gameplay Cue")]
    public class GameplayCue : ScriptableObject {

        [SerializeField] protected BaseGameplayCueAction ExecuteAction;

        [SerializeField] protected BaseGameplayCueAction OnActiveAction;

        [SerializeField] protected BaseGameplayCueAction WhileActiveAction;

        [SerializeField] protected BaseGameplayCueAction OnRemoveAction;

        public void HandleGameplayCue(GameObject Target, GameplayCueParameters Parameters, EGameplayCueEvent Event) {
            switch (Event) {
                case EGameplayCueEvent.OnExecute:
                    if (ExecuteAction == null) break;
                    ExecuteAction.Action(Target, Parameters);
                    break;
                case EGameplayCueEvent.OnActive:
                    if (OnActiveAction == null) break;
                    OnActiveAction.Action(Target, Parameters);
                    break;
                case EGameplayCueEvent.WhileActive:
                    if (WhileActiveAction == null) break;
                    WhileActiveAction.Action(Target, Parameters);
                    break;
                case EGameplayCueEvent.OnRemove:
                    if (OnRemoveAction == null) break;
                    OnRemoveAction.Action(Target, Parameters);
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
