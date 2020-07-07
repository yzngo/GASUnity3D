using System;
using GAS;
using GAS.Enums;
using GAS.Abilities;

namespace GAS.Statics {
    public class AbilitySystemStatics {
        public static void SendGameplayEventToComponent(AbilitySystemComponent TargetAbilitySystem, GameplayTag EventTag, GameplayEventData Payload) {
            TargetAbilitySystem.HandleGameplayEvent(EventTag, Payload);
        }
    }
}
