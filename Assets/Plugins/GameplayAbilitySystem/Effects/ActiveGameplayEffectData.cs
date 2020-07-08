using UnityEngine;
using System;
using GAS.Interfaces;
using System.Collections.Generic;
using GAS.Attributes;

namespace GAS.GameplayEffects {

    /// 激活的游戏效果的数据集, 用于跟踪活动中的effect
    [Serializable]
    public class ActiveGameplayEffectData {
        private GameplayEffect gameplayEffect = default;
        private float startWorldTime;                  //激活时间, Time.time

        /// The actual GameplayEffect
        public GameplayEffect Effect { get => gameplayEffect; }
        public float StartWorldTime { get => startWorldTime; }
        public IGameplayAbilitySystem Instigator { get; private set; }      // 发起者
        public IGameplayAbilitySystem Target { get; private set; }          // 目标对象

        public ActiveGameplayEffectData(GameplayEffect effect, IGameplayAbilitySystem instigator, IGameplayAbilitySystem target) {
            gameplayEffect = effect;
            this.startWorldTime = Time.time;
            this.Instigator = instigator;
            this.Target = target;
            if (!this.Effect.Period.ExecuteOnApplication) {
                this._timeOfLastPeriodicApplication = Time.time;
            }
        }

        bool _bForceRemoveEffect = false;

        public bool bForceRemoveEffect => _bForceRemoveEffect;

        /// <summary>
        /// The cooldown time that has already elapsed for this gameplay effect
        /// </summary>
        /// <value>Cooldown time elapsed</value>
        public float CooldownTimeElapsed { get => Time.time - startWorldTime; }

        /// <summary>
        /// The total cooldown time for this gameplay effect
        /// </summary>
        /// <value>Cooldown time total</value>
        public float CooldownTimeTotal { get => Effect.GameplayEffectPolicy.DurationPolicy == DurationPolicy.Duration ? Effect.GameplayEffectPolicy.DurationMagnitude : 0; }

        /// <summary>
        /// The cooldown time that is remaining for this gameplay effect
        /// </summary>
        /// <value>Cooldown time remaining</value>
        public float CooldownTimeRemaining { get => Effect.GameplayEffectPolicy.DurationPolicy == DurationPolicy.Duration ? CooldownTimeTotal - CooldownTimeElapsed : 0; }

        private float _timeOfLastPeriodicApplication = 0;

        public float TimeSincePreviousPeriodicApplication { get => Time.time - _timeOfLastPeriodicApplication; }
        public float TimeUntilNextPeriodicApplication { get => _timeOfLastPeriodicApplication + Effect.Period.Period - Time.time; }

        private Dictionary<AttributeType, Aggregator> PeriodicEffectModificationsToDate = new Dictionary<AttributeType, Aggregator>();


        [SerializeField] private int _stacks;



        public void CheckOngoingTagRequirements() {

        }


        /// <summary>
        /// Reset duration of this effect.
        /// Optionally, we can provide an offset to compensate for
        /// the fact that the reset did not happen at exactly 0
        /// and over time this could cause time drift
        /// </summary>
        /// <param name="offset">Overflow time</param>
        public void ResetDuration(float offset = 0) {
            this.startWorldTime = Time.time;
        }

        public void EndEffect() {
            this.startWorldTime = Time.time - CooldownTimeTotal;
        }

        public void ForceEndEffect() {
            EndEffect();
            _bForceRemoveEffect = true;
        }

        /// <summary>
        /// Reset time at which last periodic application occured.
        /// Optionally, we can provide an offset to compensate for
        /// the fact that the reset did not happen at exactly 0
        /// and over time this could cause time drift
        /// </summary>
        /// <param name="offset">Overflow time</param>
        public void ResetPeriodicTime(float offset = 0) {
            this._timeOfLastPeriodicApplication = Time.time - offset;
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
                var aggregators = Target.ActiveGameplayEffectsContainer.ActiveEffectAttributeAggregator
                                    .GetAggregatorsForAttribute(modifier.Attribute);
                Target.ActiveGameplayEffectsContainer.UpdateAttribute(aggregators, modifier.Attribute);
            }
        }

        public Aggregator GetPeriodicAggregatorForAttribute(AttributeType Attribute) {
            PeriodicEffectModificationsToDate.TryGetValue(Attribute, out var aggregator);
            return aggregator;
        }
    }
}

