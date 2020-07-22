using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class EffectConfigs {
        public int Id;
        public Sprite Icon;
        public EffectType EffectType;
        public DurationConfig DurationConfig;
        public PeriodConfig PeriodConfig;
        public StackConfig StackConfig;
        public List<ModifierConfig> Modifiers;
        public List<RemoveEffectInfo> RemoveEffectsInfo;
        public List<EffectCues> Cues;
    }
// Type ----------------------------------------------------------------------------------
    [Serializable]
    public enum EffectType {
        Normal,
        Cost,
        CoolDown,
        GlobalCoolDown
    }
// Duration ------------------------------------------------------------------------------

    [Serializable]
    public class DurationConfig {
        public DurationPolicy Policy;
        public float DurationLength;
    }

    public enum DurationPolicy {
        Instant,
        Duration,
        Infinite
    }

// Period --------------------------------------------------------------------------------

    [Serializable]
    public class PeriodConfig {
        public float Period;
        public bool IsExecuteOnApply;
        public Effect EffectOnExecute;
    }

// Stack ---------------------------------------------------------------------------------

    [Serializable]
    public class StackConfig {
        public StackType Type;

        [FormerlySerializedAs("Limit")]
        public int MaxStacks;
        public StackExpirationPolicy ExpirationPolicy;     // 最上边一层到期时执行的策略 
    }

    public enum StackType {
        None, 
        StackBySource, 
        StackByTarget
    }

    public enum StackExpirationPolicy {
        ClearEntireStack,                       // 清空整个栈
        RemoveSingleStackAndRefreshDuration,    // 移除一个元素且刷新时间
        RefreshDuration                         // 只刷新时间, 不移除, 即永不过期, 无限循环
    }

// Modifier Config -----------------------------------------------------------------------

    [Serializable]
    public class ModifierConfig
    {
        [FormerlySerializedAs("Type")]
        public string AttributeType;
        public OperationType OperationType;
        public float Value;
    }

    public enum OperationType 
    {
        Add, 
        Multiply, 
        Divide,
        Override
    }

// Remove Effect Info --------------------------------------------------------------------

    [Serializable]
    public class RemoveEffectInfo {
        [Tooltip("GameplayEffects with this id will be candidates for removal")]
        public int RemoveId;

        [Tooltip("Number of stacks of each GameEffect to remove.  0 means remove all stacks.")]
        public int RemoveStacks = 0;
    }
}
