using UnityEngine;
using AbilitySystem;
namespace AbilitySystem.Cues {
    /// <summary>
    /// Custom GameplayCue methods should derive from this and override the Action() method
    /// </summary>
    public abstract class BaseCueAction : ScriptableObject {

        public virtual void Action(AbilitySystemComponent target) {
        }
    }
}
