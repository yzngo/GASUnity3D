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

// time
        public float StartTime { get; private set; }

        // The time that has already elapsed for this effect
        public float ElapsedTime => Time.time - StartTime;

        // The total time for this effect
        public float TotalTime => Effect.Configs.DurationConfig.Policy == DurationPolicy.Duration ? Effect.Configs.DurationConfig.DurationLength : 0;

        // The time that is remaining for this effect
        public float RemainingTime => Effect.Configs.DurationConfig.Policy == DurationPolicy.Duration ? TotalTime - ElapsedTime : 0;

        // Reset start time of this effect.
        public void ResetStartTime(float offset = 0) => StartTime = Time.time - offset;

//------------------------------------------
        // 若强制移除, 则 = true ?
        public bool ForceRemoveEffect { get; private set; }
        public void EndEffect() 
        {
            StartTime = Time.time - TotalTime;
        }

        public void ForceEndEffect() 
        {
            EndEffect();
            ForceRemoveEffect = true;
        }

// period
        private float timeOfLastPeriodicApply;
        public float TimeSincePreviousPeriodicApply => Time.time - timeOfLastPeriodicApply;
        public float TimeUntilNextPeriodApply => timeOfLastPeriodicApply + Effect.Configs.PeriodConfig.Period - Time.time;
        private Dictionary<string, AttributeOperationContainer> periodicOperation = new Dictionary<string, AttributeOperationContainer>();

        /// Reset time at which last periodic application occured.
        public void ResetPeriodicTime(float offset = 0) => timeOfLastPeriodicApply = Time.time - offset;

        public void AddPeriodicOperation() 
        {
            foreach (ModifierConfig modifier in Effect.Configs.Modifiers) {

                if (!periodicOperation.TryGetValue(modifier.AttributeType, out var aggregator)) {
                    aggregator = new AttributeOperationContainer();
                    periodicOperation.Add(modifier.AttributeType, aggregator);
                }

                aggregator.AddOperation(modifier.OperationType, modifier.Value);

                // Recalculate new value by recomputing all aggregators
                var aggregators = Target.ActivedEffects
                                    .GetAllOperationFor(modifier.AttributeType);
                Target.ActivedEffects.UpdateAttribute(aggregators, modifier.AttributeType);
            }
        }

        public AttributeOperationContainer GetPeriodicAggregatorForAttribute(string Attribute) 
        {
            periodicOperation.TryGetValue(Attribute, out var aggregator);
            return aggregator;
        }

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

