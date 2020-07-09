using System.Collections.Generic;

namespace GameplayAbilitySystem.GameplayEffects {
    //多个标签的容器
    public abstract class GameplayEffectTagContainer {
        public abstract bool HasAny(IEnumerable<GameplayTag> Tags);
        public abstract bool HasAll(IEnumerable<GameplayTag> Tags);
    }
}