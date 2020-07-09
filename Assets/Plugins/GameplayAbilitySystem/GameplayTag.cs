using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilitySystem 
{
    // Gameplay tags are used to define how various aspects interact with each other
    [Serializable,CreateAssetMenu(fileName = "Gameplay Tag", menuName = "Ability System/Gameplay Tag")]
    public class GameplayTag : ScriptableObject 
    {
        // A static container for keeping track of all Tag that are used.
        public static HashSet<GameplayTag> GameplayTags = new HashSet<GameplayTag>();

        // A developer friendly comment
        public string Comment;

        void OnEnable() {
            // When this Tag is initialized, add the instance to the static container
            if (!GameplayTags.Contains(this)) {
                GameplayTags.Add(this);
            }
        }

        void OnDisable() {
            /// When this Tag is destroyed, remove the instance from the static container
            GameplayTags.Remove(this);
        }

    }
}
