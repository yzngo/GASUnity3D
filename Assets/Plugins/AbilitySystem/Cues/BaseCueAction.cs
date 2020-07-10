using UnityEngine;
using GameplayAbilitySystem;
namespace GameplayAbilitySystem.Cues {

    // Custom GameplayCue methods should derive from this and override the Action() method
    public abstract class BaseCueAction : ScriptableObject {

        public virtual void Action(AbilitySystem target) {
        }
    }
}
