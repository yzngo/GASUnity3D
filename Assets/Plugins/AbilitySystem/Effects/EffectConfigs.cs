using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class EffectConfigs 
    {
        public int Id;
        public Sprite Icon;
        public EffectType EffectType;
        public DurationConfig DurationConfig;
        public PeriodConfig PeriodConfig;
        public StackConfig StackConfig;
        public List<ModifierConfig> Modifiers;
        public List<RemoveEffectInfo> RemoveEffectsInfo;
        public EffectCues EffectCues;
    }

// Type ----------------------------------------------------------------------------------

    [Serializable]
    public enum EffectType 
    {
        Normal,
        Cost,
        CoolDown,
        GlobalCoolDown
    }
// Duration ------------------------------------------------------------------------------

    [Serializable]
    public struct DurationConfig 
    {
        public DurationPolicy Policy;
        public float Duration;

        public DurationConfig(DurationPolicy policy, float duration)
        {
            Policy = policy;
            Duration = duration;
        }
    }

    public enum DurationPolicy 
    {
        Instant,
        Duration,
        Infinite
    }

// Period --------------------------------------------------------------------------------

    [Serializable]
    public struct PeriodConfig 
    {
        public float Period;
        public bool IsExecuteOnApply;
        public Effect EffectOnExecute;
        public PeriodConfig(float period, bool isExecuteOnApply, Effect effectOnExecute)
        {
            Period = period;
            IsExecuteOnApply = isExecuteOnApply;
            EffectOnExecute = effectOnExecute;
        }
    }

// Stack ---------------------------------------------------------------------------------

    [Serializable]
    public struct StackConfig 
    {
        public StackType Type;
        public int MaxStacks;
        public StackExpirationPolicy ExpirationPolicy;     // 最上边一层到期时执行的策略 

        public StackConfig(StackType type, int maxStacks, StackExpirationPolicy policy)
        {
            Type = type;
            MaxStacks = maxStacks;
            ExpirationPolicy = policy;
        }
    }

    public enum StackType 
    {
        None, 
        StackBySource, 
        StackByTarget
    }

    public enum StackExpirationPolicy 
    {
        ClearEntireStack,                       // 清空整个栈
        RemoveSingleStackAndRefreshDuration,    // 移除一个元素且刷新时间
        RefreshDuration                         // 只刷新时间, 不移除, 即永不过期, 无限循环
    }

// Modifier Config -----------------------------------------------------------------------

    [Serializable]
    public struct ModifierConfig
    {
        public string AttributeType;
        public OperationType OperationType;
        public float Value;

        public ModifierConfig(string type, OperationType operation, float value)
        {
            AttributeType = type;
            OperationType = operation;
            Value = value;
        }
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
    public struct RemoveEffectInfo 
    {
        [Tooltip("GameplayEffects with this id will be candidates for removal")]
        public int RemoveId;

        [Tooltip("Number of stacks of each GameEffect to remove.  0 means remove all stacks.")]
        public int RemoveStacks;
    }

// cues ----------------------------------------------------------------------------------

    [Serializable]
    public struct EffectCues
    {
        public BaseCueAction OnActiveAction;
        public BaseCueAction OnExecuteAction;
        public BaseCueAction OnRemoveAction;

        public EffectCues(BaseCueAction onActive, BaseCueAction onExecute, BaseCueAction onRemove)
        {
            OnActiveAction = onActive;
            OnExecuteAction = onExecute;
            OnRemoveAction = onRemove;
        }

        public void HandleCue(AbilitySystem target, CueEventMomentType moment) {
            switch (moment) {
                case CueEventMomentType.OnActive:
                    OnActiveAction?.Execute(target);
                    break;
                case CueEventMomentType.OnExecute:
                    OnExecuteAction?.Execute(target);
                    break;
                case CueEventMomentType.OnRemove:
                    OnRemoveAction?.Execute(target);
                    break;
            }
        }
    }

    public enum CueEventMomentType 
    {
        OnActive,       // Called when Cue is first activated
        OnExecute,      // Called when a Cue is executed (e.g. instant/periodic/tick)
        OnRemove        // Called when a Cue is removed
    }
}

