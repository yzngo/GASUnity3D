using System;
using UnityEngine;

namespace GAS.GameplayCues {
    public abstract class AbstractGameplayCueNotify_Actor {
        public abstract void Execute(GameObject Target, GameplayCueParameters Parameters);
    }
}
