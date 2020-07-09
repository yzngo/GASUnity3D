using UnityEngine;
using AbilitySystem;
namespace AbilitySystem.Cues {

    // Custom GameplayCue methods should derive from this and override the Action() method
    public abstract class BaseCueAction : ScriptableObject {

        public virtual void Action(AbilitySystemComponent target) {
        }
    }
}
