using UnityEngine;

namespace AbilitySystem.Cues {
    /// <summary>
    /// Custom GameplayCue methods should derive from this and override the Action() method
    /// </summary>
    public abstract class BaseGameplayCueAction : ScriptableObject {
        public virtual void Action(GameObject target) {

        }
    }
}
