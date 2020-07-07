using System.Linq;
using System;
using GAS.Interfaces;
using GAS.GameplayEffects;
using UnityEngine;
using System.Collections.Generic;

namespace GAS {
    /// <inheritdoc />
    [Serializable]
    public class GameplayCooldown : IGameplayCooldown {
        /// <inheritdoc />
        public GameplayEffect CooldownGameplayEffect => _cooldownGameplayEffect;

        /// <inheritdoc />
        [SerializeField]
        private GameplayEffect _cooldownGameplayEffect = null;

        /// <inheritdoc />
        public IEnumerable<GameplayTag> GetCooldownTags() {
            return _cooldownGameplayEffect.GameplayEffectTags.GrantedTags.Added;
        }
    }
}
