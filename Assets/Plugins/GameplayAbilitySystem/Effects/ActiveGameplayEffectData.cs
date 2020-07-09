using UnityEngine;
using System;
using GameplayAbilitySystem.Interfaces;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes;

namespace GameplayAbilitySystem.Effects {

    /// 激活的某个游戏效果的数据集, 用于跟踪活动中的effect
    [Serializable]
    public class ActiveGameplayEffectData {
// base data
        private GameplayEffect gameplayEffect = default;
        private float startWorldTime;                  //激活时间, Time.time
        private float timeOfLastPeriodicApplication = 0;

        /// The actual GameplayEffect
        public GameplayEffect Effect => gameplayEffect;
        public float StartWorldTime => startWorldTime;
        public AbilitySystem Instigator { get; private set; }      // 发起者
        public AbilitySystem Target { get; private set; }          // 目标对象
// ctor
        public ActiveGameplayEffectData(GameplayEffect effect, AbilitySystem instigator, AbilitySystem target) {
            gameplayEffect = effect;
            startWorldTime = Time.time;
            Instigator = instigator;
            Target = target;

            if (!Effect.Periodicity.ExecuteOnApplication) {
                timeOfLastPeriodicApplication = Time.time;
            }
        }

// force remove?
        // 若强制移除, 则 = true
        private bool forceRemoveEffect = false;
        public bool ForceRemoveEffect => forceRemoveEffect;

// cooldown time
        /// The cooldown time that has already elapsed for this gameplay effect
        public float CooldownTimeElapsed => Time.time - startWorldTime;
        /// <summary>
        /// The total cooldown time for this gameplay effect
        /// </summary>
        /// <value>Cooldown time total</value>
        public float CooldownTimeTotal => 
                Effect.GameplayEffectPolicy.DurationPolicy == DurationPolicy.Duration ? 
                                            Effect.GameplayEffectPolicy.DurationMagnitude : 0;
        /// <summary>
        /// The cooldown time that is remaining for this gameplay effect
        /// </summary>
        /// <value>Cooldown time remaining</value>
        public float CooldownTimeRemaining => 
                Effect.GameplayEffectPolicy.DurationPolicy == DurationPolicy.Duration ? 
                                            CooldownTimeTotal - CooldownTimeElapsed : 0;

// period
        // 对于周期性的effect而言, 自从上次应用效果之后流逝的时间
        public float TimeSincePreviousPeriodicApplication => Time.time - timeOfLastPeriodicApplication;
        // 对于周期性的effect而言, 到下次应用还需要的时间
        public float TimeUntilNextPeriodicApplication => timeOfLastPeriodicApplication + Effect.Periodicity.Period - Time.time;
        private Dictionary<AttributeType, Aggregator> PeriodicEffectModificationsToDate = new Dictionary<AttributeType, Aggregator>();

// reset time
        /// Reset duration of this effect.
        /// Optionally, we can provide an offset to compensate for
        /// the fact that the reset did not happen at exactly 0
        /// and over time this could cause time drift
        public void ResetDuration(float offset = 0) {
            this.startWorldTime = Time.time - offset;
        }

        /// Reset time at which last periodic application occured.
        public void ResetPeriodicTime(float offset = 0) {
            this.timeOfLastPeriodicApplication = Time.time - offset;
        }

        public void EndEffect() {
            this.startWorldTime = Time.time - CooldownTimeTotal;
        }

        public void ForceEndEffect() {
            EndEffect();
            forceRemoveEffect = true;
        }


        public void AddPeriodicEffectAttributeModifiers() {
            // Check out ActiveGameplayEffectContainer.AddActiveGameplayEffect to see how to populate the ActiveEffectAttributeAggregator object
            foreach (var modifier in Effect.GameplayEffectPolicy.Modifiers) {
                modifier.AttemptCalculateMagnitude(out var EvaluatedMagnitude);

                // If aggregator for this attribute doesn't exist, add it.
                if (!PeriodicEffectModificationsToDate.TryGetValue(modifier.Attribute, out var aggregator)) {
                    aggregator = new Aggregator(modifier.Attribute);
                    // aggregator.Dirtied.AddListener(UpdateAttribute);
                    PeriodicEffectModificationsToDate.Add(modifier.Attribute, aggregator);
                }

                aggregator.AddAggregatorModifier(EvaluatedMagnitude, modifier.ModifierOperation);

                // Recalculate new value by recomputing all aggregators
                var aggregators = Target.ActiveEffectsContainer.ActiveEffectAttributeAggregator
                                    .GetAggregatorsForAttribute(modifier.Attribute);
                Target.ActiveEffectsContainer.UpdateAttribute(aggregators, modifier.Attribute);
            }
        }

        public Aggregator GetPeriodicAggregatorForAttribute(AttributeType Attribute) {
            PeriodicEffectModificationsToDate.TryGetValue(Attribute, out var aggregator);
            return aggregator;
        }

        // [SerializeField] private int _stacks;
        // public void CheckOngoingTagRequirements() {
        // }
    }
}

