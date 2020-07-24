using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem
{
    [Serializable]
    public class EffectConfigs 
    {
        public string Id;
        public bool DisplayWhenActived;
        public EffectType EffectType;
        public DurationConfig DurationConfig;
        public PeriodConfig PeriodConfig;
        public StackConfig StackConfig;
        public List<ModifierConfig> Modifiers;
        public List<RemoveEffectInfo> RemoveEffectsInfo;
        public EffectCues EffectCues;

        private static Dictionary<string, EffectConfigs> configs = new Dictionary<string, EffectConfigs>();

        public static EffectConfigs GetCoolDownConfig(string abilityId) 
        {
            string effectId = abilityId + "CoolDownEffect";
            if (!configs.TryGetValue( effectId, out var config )) {
                config = new EffectConfigs();
                configs.Add(effectId, config);
                config.Id = effectId;
                config.EffectType = EffectType.CoolDown;
                // 根据ability表获取cd时长
                // -----------------------------------------------------
                float duration = 0;
                if (abilityId == ID.ability_fire ) {
                    duration = 1;
                } else if(abilityId == ID.ability_heal) {
                    duration = 2;
                } else if(abilityId == ID.ability_bloodPact) {
                    duration = 3;
                }
                // -----------------------------------------------------
                config.DurationConfig = new DurationConfig(DurationPolicy.Duration, duration);
            }
            return config;
        }

        public static EffectConfigs GetGlobalCoolDownConfig()
        {
            string effectId = "globalCoolDownEffect";
            if (!configs.TryGetValue(effectId, out var config)) {
                config = new EffectConfigs();
                configs.Add(effectId, config);
                config.Id = effectId;
                config.EffectType = EffectType.GlobalCoolDown;
                // 读配置获取公共cd时长
                // --------------------------------------------------------
                float duration = 1;
                // --------------------------------------------------------
                config.DurationConfig = new DurationConfig(DurationPolicy.Duration, duration);
            }
            return config;   
        }

        public static EffectConfigs GetCostConfig(string abilityId)
        {
            string effectId = abilityId + "Cost";
            if (!configs.TryGetValue(abilityId, out var config)) {
                config = new EffectConfigs();
                configs.Add(effectId, config);
                config.Id = effectId;
                config.EffectType = EffectType.Cost;

                // 根据ability表获取cost信息
                // --------------------------------------------------------
                config.Modifiers = new List<ModifierConfig>();
                if (abilityId == ID.ability_fire) {
                    config.Modifiers.Add( new ModifierConfig(AttributeType.Mana, OperationType.Add, -5));

                } else if (abilityId == ID.ability_bloodPact) {
                    config.Modifiers.Add( new ModifierConfig(AttributeType.Health, OperationType.Add, -5));

                } else if (abilityId == ID.ability_heal) {
                    config.Modifiers.Add( new ModifierConfig(AttributeType.Mana, OperationType.Add, -10));
                }
                // ---------------------------------------------------------
            }
            return config;
        }

        public static EffectConfigs GetNormalConfig(string effectId)
        {
            if (!configs.TryGetValue(effectId, out var config)) {
                config = new EffectConfigs();
                configs.Add(effectId, config);
                config.Id = effectId;
                config.EffectType = EffectType.Normal;

                //根据effect表配置effect config
                // ----------------------------------------------------------
                if (effectId == ID.effect_regenHealth) {
                    config.Modifiers = new List<ModifierConfig>() {
                        new ModifierConfig(AttributeType.Health, OperationType.Add, 10)
                    };
                    config.EffectCues = new EffectCues(
                        SpawnCueAction.Get(ID.cue_regenHealthSprite),
                        SpawnCueAction.Get(ID.cue_regenHealthSprite),
                        null
                    );

                } else if(effectId == ID.effect_regenMana) {
                    
                    config.Modifiers = new List<ModifierConfig>() {
                        new ModifierConfig(AttributeType.Mana, OperationType.Add, 10)
                    };
                    config.EffectCues = new EffectCues(
                        SpawnCueAction.Get(ID.cue_manaSurgeZ),
                        SpawnCueAction.Get(ID.cue_manaSurgeZ),
                        null
                    );
                
                } else if(effectId == ID.effect_heal) {
                    config.DisplayWhenActived = true;
                    config.DurationConfig = new DurationConfig(DurationPolicy.Duration, 20);
                    config.PeriodConfig = new PeriodConfig(3, false, ID.effect_regenHealth);
                    config.RemoveEffectsInfo = new List<RemoveEffectInfo>() {
                        new RemoveEffectInfo(ID.effect_bloodPact, 1)
                    };
                    config.EffectCues = new EffectCues(SpawnCueAction.Get(ID.cue_spawnMagicCircle), null, null);

                } else if(effectId == ID.effect_fire) {
                    config.Modifiers = new List<ModifierConfig>() {
                        new ModifierConfig(AttributeType.Health, OperationType.Add, -5),
                    };
                    config.EffectCues = new EffectCues(
                        SpawnCueAction.Get(ID.cue_spawnBigExplosion),
                        SpawnCueAction.Get(ID.cue_spawnBigExplosion),
                        SpawnCueAction.Get(ID.cue_spawnBigExplosion)
                    );

                } else if(effectId == ID.effect_bloodPact) {
                    config.DisplayWhenActived = true;
                    config.DurationConfig = new DurationConfig(DurationPolicy.Duration, 20);
                    config.PeriodConfig = new PeriodConfig(2, true, ID.effect_regenMana);
                    config.StackConfig = new StackConfig(StackType.StackBySource, 2, StackExpirationPolicy.RemoveSingleStackAndRefreshDuration);
                    config.EffectCues = new EffectCues(
                        SpawnCueAction.Get(ID.cue_spawnEnergyExplosion),
                        null,
                        SpawnCueAction.Get(ID.cue_spawnEnergyExplosion)
                    );
                }
                // ---------------------------------------------------------------
            }
            return config;
        }
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
        public string EffectId;

        public Effect EffectOnExecute;
        public PeriodConfig(float period, bool isExecuteOnApply, string effectId)
        {
            Period = period;
            IsExecuteOnApply = isExecuteOnApply;
            EffectId = effectId;
            EffectOnExecute = Effect.Get(EffectConfigs.GetNormalConfig(effectId));
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
        public string RemoveId;

        [Tooltip("Number of stacks of each GameEffect to remove.  0 means remove all stacks.")]
        public int RemoveStacks;
        public RemoveEffectInfo(string removeId, int removeStacks)
        {
            RemoveId = removeId;
            RemoveStacks = removeStacks;
        }
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

