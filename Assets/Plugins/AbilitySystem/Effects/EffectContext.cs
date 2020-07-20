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
        public float StartTime { get; private set; }
        public AbilitySystem Instigator { get; private set; }
        public AbilitySystem Target { get; private set; }

        private float timeOfLastPeriodicApply;

        public EffectContext(int sourceId, Effect effect, AbilitySystem instigator, AbilitySystem target) 
        {
            SourceId = sourceId;
            Effect = effect;
            StartTime = Time.time;
            Instigator = instigator;
            Target = target;

            if (!Effect.Configs.PeriodConfig.IsExecuteOnApply) {
                timeOfLastPeriodicApply = Time.time;
            }
        }

        public bool IsCoolDownOf(Ability ability) 
        {
            if (Effect.Configs.EffectType == EffectType.GlobalCoolDown) {
                return true;
            }
            if (ability.Id == SourceId && Effect.Configs.EffectType == EffectType.CoolDown) {
                return true;
            }
            return false;
        }

// force remove?
        // 若强制移除, 则 = true
        private bool forceRemoveEffect = false;
        public bool ForceRemoveEffect => forceRemoveEffect;

// cooldown time
        /// The cooldown time that has already elapsed for this gameplay effect
        public float ElapsedTime => Time.time - StartTime;
        /// <summary>
        /// The total cooldown time for this gameplay effect
        /// </summary>
        /// <value>Cooldown time total</value>
        public float TotalTime => 
                Effect.Configs.DurationConfig.Policy == DurationPolicy.Duration ? Effect.Configs.DurationConfig.DurationLength : 0;
        /// <summary>
        /// The cooldown time that is remaining for this gameplay effect
        /// </summary>
        /// <value>Cooldown time remaining</value>
        public float RemainingTime => 
                Effect.Configs.DurationConfig.Policy == DurationPolicy.Duration ? TotalTime - ElapsedTime : 0;

// period
        // 对于周期性的effect而言, 自从上次应用效果之后流逝的时间
        public float TimeSincePreviousPeriodApply => Time.time - timeOfLastPeriodicApply;
        // 对于周期性的effect而言, 到下次应用还需要的时间
        public float TimeUntilNextPeriodApply => timeOfLastPeriodicApply + Effect.Configs.PeriodConfig.Period - Time.time;
        private Dictionary<AttributeType, AttributeModifyAggregator> PeriodicEffectModificationsToDate = new Dictionary<AttributeType, AttributeModifyAggregator>();

// reset time
        /// Reset duration of this effect.
        /// Optionally, we can provide an offset to compensate for
        /// the fact that the reset did not happen at exactly 0
        /// and over time this could cause time drift
        public void ResetDuration(float offset = 0) => StartTime = Time.time - offset;

        /// Reset time at which last periodic application occured.
        public void ResetPeriodicTime(float offset = 0) => timeOfLastPeriodicApply = Time.time - offset;

        public void EndEffect() 
        {
            StartTime = Time.time - TotalTime;
        }

        public void ForceEndEffect() 
        {
            EndEffect();
            forceRemoveEffect = true;
        }

        public void AddPeriodicEffectAttributeModifiers() 
        {
            // Check out ActiveGameplayEffectContainer.AddActiveGameplayEffect to see how to populate the ActiveEffectAttributeAggregator object
            foreach (var modifier in Effect.Configs.Modifiers) {
                modifier.AttemptCalculateMagnitude(out var evaluatedValue);

                // If aggregator for this attribute doesn't exist, add it.
                if (!PeriodicEffectModificationsToDate.TryGetValue(modifier.AttributeType, out var aggregator)) {
                    aggregator = new AttributeModifyAggregator(modifier.AttributeType);
                    // aggregator.Dirtied.AddListener(UpdateAttribute);
                    PeriodicEffectModificationsToDate.Add(modifier.AttributeType, aggregator);
                }

                aggregator.AddAggregatorModifier(modifier.OperationType, evaluatedValue);

                // Recalculate new value by recomputing all aggregators
                var aggregators = Target.EffectsContainer
                                    .GetAggregatorsForAttribute(modifier.AttributeType);
                Target.EffectsContainer.UpdateAttribute(aggregators, modifier.AttributeType);
            }
        }

        public AttributeModifyAggregator GetPeriodicAggregatorForAttribute(AttributeType Attribute) 
        {
            PeriodicEffectModificationsToDate.TryGetValue(Attribute, out var aggregator);
            return aggregator;
        }
    }
}

