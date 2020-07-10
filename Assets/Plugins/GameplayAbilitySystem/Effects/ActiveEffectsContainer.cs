using System.Linq;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes;
using System;
using UniRx.Async;
using System.Threading.Tasks;
using GameplayAbilitySystem.Cues;

namespace GameplayAbilitySystem.Effects 
{
    [Serializable]
    public class EffectsContainer 
    {
        private AbilitySystem target;

        /// <summary>
        /// This is used to keep track of all the "temporary" attribute modifiers,
        /// so we can calculate them all as f(Base, Added, Multiplied, Divided) = (Base + Added) * (Multiplied/Divided)
        /// </summary>
        /// <value></value>
        public EffectsModifyAggregator effectsModifyAggregator { get; } = new EffectsModifyAggregator();

        public EffectsContainer(AbilitySystem target) 
        {
            this.target = target;
        }
        // private ActiveGameplayEffectsEvent ActiveGameplayEffectAddedEvent = new ActiveGameplayEffectsEvent();

        //todo async 异步?
        // public async Task<ActiveGameplayEffectData> ApplyGameEffect(ActiveGameplayEffectData EffectData) {
        public void ApplyDurationalEffect(ActivedEffectData effectData) {
            // Durational effect.  Add granted modifiers to active list
            var existingStacks = -1;
            var maxStacks = effectData.Effect.StackingPolicy.StackLimit;
            IEnumerable<ActivedEffectData> matchingStackedActiveEffects = GetMatchingStackedEffectsByEffect(effectData);

            switch (effectData.Effect.StackingPolicy.StackDurationRefreshPolicy) {
                case StackRefreshPolicy.RefreshOnSuccessfulApplication: // We refresh all instances of this game effect
                    if (matchingStackedActiveEffects == null) break;
                    foreach (var effect in matchingStackedActiveEffects) {
                        effect.ResetDuration();
                    }
                    break;
                case StackRefreshPolicy.NeverRefresh: // Don't do anything.  Effect will expire naturally
                    break;

            }

            switch (effectData.Effect.StackingPolicy.StackPeriodResetPolicy) {
                case StackRefreshPolicy.RefreshOnSuccessfulApplication: // We refresh all instances of this game effect
                    if (matchingStackedActiveEffects == null) break;
                    foreach (var effect in matchingStackedActiveEffects) {
                        effect.ResetPeriodicTime();
                    }
                    break;
                case StackRefreshPolicy.NeverRefresh: // Don't do anything.  Each stack maintains its own periodic effect
                    break;
            }


            existingStacks = matchingStackedActiveEffects?.Count() ?? -1;
            if (existingStacks < maxStacks) { // We can still add more stacks.
                AddActiveGameplayEffect(effectData);
                // ActiveGameplayEffectAddedEvent?.Invoke(AbilitySystem, EffectData);

                // We only need to do timed checks for durational abilities
                if (effectData.Effect.Policy.DurationPolicy == DurationPolicy.Duration
                    || effectData.Effect.Policy.DurationPolicy == DurationPolicy.Infinite) {
                    // var removalTime = EffectData.Effect.GameplayEffectPolicy.DurationMagnitude * 1000.0f;
                    CheckGameplayEffectForTimedEffects(effectData);
                }

            }
        }

        private void OnActiveGameplayEffectAdded(ActivedEffectData effectData) {
            // ActiveGameplayEffectAddedEvent?.Invoke(AbilitySystem, effectData);
        }

        private void ModifyActiveGameplayEffect(ActivedEffectData effectData, Action<GameplayEffectModifier> action) {
            foreach (var modifier in effectData.Effect.Policy.Modifiers) {
                action(modifier);
            }

            // If there are no gameplay effect modifiers, we need to add or get an empty entry
            if (effectData.Effect.Policy.Modifiers.Count == 0) {
                action((new GameplayEffectModifier()).InitializeEmpty());
            }
        }

        private void AddActiveGameplayEffect(ActivedEffectData effectData) {
            ModifyActiveGameplayEffect(effectData, modifier => {
                // We only apply if the effect has execute on application
                modifier.AttemptCalculateMagnitude(out var evaluatedValue);

                // Check if we already have an entry for this gameplay effect attribute modifier
                var attributeAggregatorMap = effectsModifyAggregator.AddorGet(effectData);

                if (modifier.AttributeType != null) {
                    // If aggregator for this attribute doesn't exist, add it.
                    if (!attributeAggregatorMap.TryGetValue(modifier.AttributeType, out AttributeModifyAggregator aggregator)) {
                        aggregator = new AttributeModifyAggregator(modifier.AttributeType);
                        attributeAggregatorMap.Add(modifier.AttributeType, aggregator);
                    }

                    // If this is a periodic effect, we don't add any attributes here. 
                    // They will be added as required on period expiry and stored in a separate structure
                    if (effectData.Effect.Periodicity.Period <= 0) {
                        aggregator.AddAggregatorModifier(modifier.ModifierOperation, evaluatedValue);
                    }

                    // Recalculate new value by recomputing all aggregators
                    var aggregators = effectsModifyAggregator.GetAggregatorsForAttribute(modifier.AttributeType);
                    UpdateAttribute(aggregators, modifier.AttributeType);
                }
            });

            // Add cooldown effect as well.  Application of cooldown effect
            // is different to other game effects, because we don't take
            // attribute modifiers into account
            OnActiveGameplayEffectAdded(effectData);
        }


        /// <summary>
        /// This function is used to do checks for things that may happen on a timed basis, such as
        /// periodic effects or effect expiry
        /// This is currently updated every frame.  Perhaps there are ways to make this more efficient?
        /// The main advantage of checking every frame is we can manipulate the WorldStartTime to
        /// effectively "refresh" the effect or end it at will.
        /// </summary>
        /// <param name="effectData"></param>
        /// <returns> </returns>
        private async Task WaitForEffectExpiryTime(ActivedEffectData effectData) {
            bool durationExpired = false;
            while (!durationExpired) {
                await UniTask.DelayFrame(0);

                if (effectData.ForceRemoveEffect) {
                    durationExpired = true;
                } else if (effectData.Effect.Policy.DurationPolicy == DurationPolicy.Duration) {
                    // Check whether required time has expired
                    // We only need to do this for effects with a finite duration
                    durationExpired = effectData.CooldownTimeRemaining <= 0 ? true : false;
                } else if (effectData.Effect.Policy.DurationPolicy == DurationPolicy.Infinite) {
                    durationExpired = effectData.StartWorldTime <= 0 ? true : false;
                }

                // Periodic effects only occur if the period is > 0
                if (effectData.Effect.Periodicity.Period > 0) {
                    CheckAndApplyPeriodicEffect(effectData);
                }

                if (durationExpired) { // This effect is due for expiry
                    ApplyStackExpirationPolicy(effectData, ref durationExpired);
                }

            }
        }

        private void CheckAndApplyPeriodicEffect(ActivedEffectData effectData) {
            if (effectData.TimeUntilNextPeriodicApplication <= 0) {
                // Apply gameplay effect defined for period.  
                if (effectData.Effect.Periodicity.EffectOnExecute != null) {
                    effectData.Instigator.ApplyEffectToTarget(effectData.Effect.Periodicity.EffectOnExecute, effectData.Target);
                }
                var gameplayCues = effectData.Effect.GameplayCues;
                foreach (var cue in gameplayCues) {
                    cue.HandleCue(effectData.Target, CueEventMomentType.OnExecute);
                }

                effectData.AddPeriodicEffectAttributeModifiers();
                effectData.ResetPeriodicTime();
            }
        }

        private void ApplyStackExpirationPolicy(ActivedEffectData effectData, ref bool durationExpired) {
            IEnumerable<ActivedEffectData> matchingEffects;

            switch (effectData.Effect.StackingPolicy.StackExpirationPolicy) {
                case StackExpirationPolicy.ClearEntireStack: // Remove all effects which match
                    matchingEffects = GetMatchingStackedEffectsByEffect(effectData);
                    if (matchingEffects == null) break;
                    foreach (var effect in matchingEffects) {
                        effect.EndEffect();
                    }
                    break;
                case StackExpirationPolicy.RemoveSingleStackAndRefreshDuration:
                    // Remove this effect, and reset all other durations to max
                    matchingEffects = GetMatchingStackedEffectsByEffect(effectData);
                    if (matchingEffects == null) break;

                    foreach (var effect in matchingEffects) {
                        // We need to cater for the fact that the cooldown
                        // may have exceeded the actual limit by a little bit
                        // due to framerate.  So, when we reset the other cooldowns
                        // we need to account for this difference
                        var timeOverflow = effect.CooldownTimeRemaining;
                        effect.ResetDuration(timeOverflow);
                    }
                    // This effect was going to expire anyway, but we put this here to be explicit to future code readers
                    effectData.EndEffect();
                    break;
                case StackExpirationPolicy.RefreshDuration:
                    // Refreshing duration on expiry basically means the effect can never expire
                    matchingEffects = GetMatchingStackedEffectsByEffect(effectData);
                    if (matchingEffects == null) break;
                    foreach (var effect in matchingEffects) {
                        effect.ResetDuration();
                        durationExpired = false; // Undo effect expiry.  This effect should never expire
                    }
                    break;
            }
        }

        private async void CheckGameplayEffectForTimedEffects(ActivedEffectData effectData) {
            await WaitForEffectExpiryTime(effectData);
            var gameplayCues = effectData.Effect.GameplayCues;
            foreach (var cue in gameplayCues) {
                cue.HandleCue(effectData.Target, CueEventMomentType.OnRemove);
            }
            // There could be multiple stacked effects, due to multiple casts
            // Remove one instance of this effect from the active list
            ModifyActiveGameplayEffect(effectData, modifier => {

                effectsModifyAggregator.RemoveEffect(effectData);
                if (modifier.AttributeType == null) return;

                // Find all remaining aggregators of the same type and recompute values
                var aggregators = effectsModifyAggregator.GetAggregatorsForAttribute(modifier.AttributeType);

                // If there are no aggregators, set base = current
                if (aggregators.Count() == 0) {
                    var current = target.GetBaseValue(modifier.AttributeType);
                    if (current < 0) target.SetBaseValue(modifier.AttributeType, 0f);
                    target.SetCurrentValue(modifier.AttributeType, current);
                } else {
                    UpdateAttribute(aggregators, modifier.AttributeType);
                }
            });

        }

        public void UpdateAttribute(IEnumerable<AttributeModifyAggregator> aggregator, AttributeType attributeType) {
            var baseAttributeValue = target.GetBaseValue(attributeType);
            var newCurrentAttributeValue = aggregator.Evaluate(baseAttributeValue);
            target.SetCurrentValue(attributeType, newCurrentAttributeValue);
        }

        private IEnumerable<ActivedEffectData> GetMatchingStackedEffectsByEffect(ActivedEffectData effectData) {
            IEnumerable<ActivedEffectData> matchingStackedActiveEffects = null;

            switch (effectData.Effect.StackingPolicy.StackingType) {
                // Stacking Type None:
                // Add effect as a separate instance. 
                case StackingType.None:
                    break;

                case StackingType.AggregatedBySource:
                    matchingStackedActiveEffects = effectsModifyAggregator
                                        .GetAllEffects()
                                        .Where(x => x.Instigator == effectData.Instigator && x.Effect == effectData.Effect);
                    break;

                case StackingType.AggregatedByTarget:
                    matchingStackedActiveEffects = effectsModifyAggregator
                                        .GetAllEffects()
                                        .Where(x => x.Target == effectData.Target && x.Effect == effectData.Effect);
                    break;
            }
            return matchingStackedActiveEffects;
        }
    }
}

