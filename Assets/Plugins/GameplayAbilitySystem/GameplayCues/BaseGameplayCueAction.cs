using UnityEngine;

namespace GAS.GameplayCues {
    /// <summary>
    /// Custom GameplayCue methods should derive from this and override the Action() method
    /// </summary>
    public class BaseGameplayCueAction : ScriptableObject {
        public virtual void Action(GameObject Target, GameplayCueParameters Parameters) {

        }
    }
}
