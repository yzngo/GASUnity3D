using UnityEngine;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes;
using GameplayAbilitySystem.Abilities;

namespace GameplayAbilitySystem.Effects 
{
    public class EffectContext 
    {
        public int SourceId { get; private set; }
        public Effect Effect { get; private set; }
        public AbilitySystem Instigator { get; private set; }
        public AbilitySystem Target { get; private set; }

        public EffectContext(int sourceId, Effect effect, AbilitySystem instigator, AbilitySystem target) 
        {
            SourceId = sourceId;
            Effect = effect;
            StartTime = Time.time;
            Instigator = instigator;
            Target = target;

            if (!Effect.Configs.PeriodConfig.IsExecuteOnApply) {
                ResetPeriodicTime();
            }
        }

// ---------------------------------------------------------------------------------------
        public float StartTime { get; private set; }

        // The duration that has already elapsed for this effect
        public float ElapsedDuration => Time.time - StartTime;

        // The total time for this effect
        public float TotalDuration => Effect.Configs.DurationConfig.Policy == DurationPolicy.Duration ? Effect.Configs.DurationConfig.Duration: 0;

        // The time that is remaining for this effect
        public float RemainingDuration => Effect.Configs.DurationConfig.Policy == DurationPolicy.Duration ? TotalDuration - ElapsedDuration : 0;

        // Reset start time of this effect.
        public void ResetStartTime(float offset = 0) => StartTime = Time.time - offset;

// ---------------------------------------------------------------------------------------
        // 若强制移除, 则 = true ?
        public bool ForceRemoveEffect { get; private set; }
        public void EndEffect() 
        {
            StartTime = Time.time - TotalDuration;
        }

        public void ForceEndEffect() 
        {
            EndEffect();
            ForceRemoveEffect = true;
        }
// ---------------------------------------------------------------------------------------
        private float periodicStartTime;
        public float PeriodicElapsedDuration => Time.time - periodicStartTime;
        public float PeriodicRemainingDuration => Effect.Configs.PeriodConfig.Period - PeriodicElapsedDuration;
        private Dictionary<string, AttributeOperationContainer> periodicOperations = new Dictionary<string, AttributeOperationContainer>();
        public void ResetPeriodicTime(float offset = 0) => periodicStartTime = Time.time - offset;

        public void ApplyPeriodicOperations() 
        {
            if (Effect.Configs.Modifiers != null) {
                foreach (ModifierConfig modifier in Effect.Configs.Modifiers) {

                    if (!periodicOperations.TryGetValue(modifier.AttributeType, out var operations)) {
                        operations = new AttributeOperationContainer();
                        periodicOperations.Add(modifier.AttributeType, operations);
                    }
                    operations.AddOperation(modifier.OperationType, modifier.Value);
                    Target.ReEvaluateCurrentValueFor(modifier.AttributeType);
                }
            }
        }

        public AttributeOperationContainer GetPeriodicOperationsFor(string attribute) 
        {
            periodicOperations.TryGetValue(attribute, out var operations);
            return operations;
        }

// ---------------------------------------------------------------------------------------
        public bool IsCoolDownEffectOf(Ability ability) 
        {
            if (Effect.Configs.EffectType == EffectType.GlobalCoolDown) {
                return true;
            }
            if (ability.Id == SourceId && Effect.Configs.EffectType == EffectType.CoolDown) {
                return true;
            }
            return false;
        }
    }
}

