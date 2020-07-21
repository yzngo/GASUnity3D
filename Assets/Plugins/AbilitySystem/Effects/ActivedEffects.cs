using System.Linq;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes;
using System;
using System.Threading.Tasks;
using GameplayAbilitySystem.Cues;
using Cysharp.Threading.Tasks;

namespace GameplayAbilitySystem.Effects 
{
    public class ActivedEffects 
    {
        private AbilitySystem target;

        // This is used to keep track of all the "temporary" attribute modifiers,
        // so we can calculate them all as f(Base, Added, Multiplied, Divided) = (Base + Added) * (Multiplied/Divided)

        // effect -> attribute -> modifies
        private Dictionary<EffectContext, Dictionary<string, AttributeModifyAggregator>> modifiesAggregator = 
            new Dictionary<EffectContext, Dictionary<string, AttributeModifyAggregator>>();


        public ActivedEffects(AbilitySystem target) 
        {
            this.target = target;
        }

        public List<EffectContext> AllEffects => modifiesAggregator.Keys.ToList();

        public void ApplyDurationalEffect(EffectContext effectContext) 
        {
            int existingStacks = -1;
            int maxStacks = effectContext.Effect.Configs.StackConfig.Limit;
            IEnumerable<EffectContext> matchingStackedActiveEffects = GetMatchingStackedEffectsByEffect(effectContext);

            switch (effectContext.Effect.Configs.StackConfig.DurationRefreshPolicy) {
                case StackRefreshPolicy.RefreshOnSuccessfulApply: // We refresh all instances of this game effect
                    if (matchingStackedActiveEffects == null) break;
                    foreach (var effect in matchingStackedActiveEffects) {
                        effect.ResetDuration();
                    }
                    break;
                case StackRefreshPolicy.NeverRefresh: // Don't do anything.  Effect will expire naturally
                    break;

            }
            switch (effectContext.Effect.Configs.StackConfig.PeriodResetPolicy) {
                case StackRefreshPolicy.RefreshOnSuccessfulApply: // We refresh all instances of this game effect
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
                ApplyEffect(effectContext);
                // We only need to do timed checks for durational abilities
                if (effectContext.Effect.Configs.DurationConfig.Policy == DurationPolicy.Duration
                    || effectContext.Effect.Configs.DurationConfig.Policy == DurationPolicy.Infinite) {
                    // var removalTime = effectContext.Effect.GameplayEffectPolicy.DurationMagnitude * 1000.0f;
                    CheckGameplayEffectForTimedEffects(effectContext);
                }
            }
        }

        public void UpdateAttribute(IEnumerable<AttributeModifyAggregator> aggregator, string attributeType) 
        {
            var baseAttributeValue = target.GetBaseValue(attributeType);
            var newCurrentAttributeValue = aggregator.Evaluate(baseAttributeValue);
            target.SetCurrentValue(attributeType, newCurrentAttributeValue);
        }


        // 获取所有effect中,修改某个attribute的所有attributeAggregator
        public IEnumerable<AttributeModifyAggregator> GetAggregatorsForAttribute(string Attribute) 
        {
            // Find all remaining aggregators of the same type and recompute values
            var aggregators = modifiesAggregator
                                .Where(x => x.Value.ContainsKey(Attribute))
                                .Select(x => x.Value[Attribute]);
            var periodic = modifiesAggregator
                            .Where(x => x.Key.Effect.Configs.PeriodConfig.Period > 0)
                            .Select(x => x.Key.GetPeriodicAggregatorForAttribute(Attribute))
                            .Where(x => x != null);
            return aggregators.Concat(periodic);
        }

        private void ModifyActiveGameplayEffect(EffectContext effectContext, Action<EffectModifier> action) 
        {
            foreach (var modifier in effectContext.Effect.Configs.Modifiers) {
                action(modifier);
            }
            // If there are no gameplay effect modifiers, we need to add or get an empty entry
            if (effectContext.Effect.Configs.Modifiers.Count == 0) {
                action((new EffectModifier()).InitializeEmpty());
            }
        }

        private void ApplyEffect(EffectContext effectContext) 
        {
            ModifyActiveGameplayEffect(effectContext, modifier => {

                // Check if we already have an entry for this gameplay effect attribute modifier
                // var attributeAggregatorMap = effectsModifyAggregator.AddorGet(effectContext);
                
                //todo-----
                if (!modifiesAggregator.TryGetValue(effectContext, out var attributeAggregators)) {
                    attributeAggregators = new Dictionary<string, AttributeModifyAggregator>();
                    modifiesAggregator.Add(effectContext, attributeAggregators);
                }
                var attributeAggregatorMap = attributeAggregators;
                // ---------------
                // Dictionary<AttributeType, AttributeModifyAggregator> dic = 

                if (!string.IsNullOrEmpty(modifier.Type)) {
                    // If aggregator for this attribute doesn't exist, add it.
                    if (!attributeAggregatorMap.TryGetValue(modifier.Type, out AttributeModifyAggregator aggregator)) {
                        aggregator = new AttributeModifyAggregator(modifier.Type);
                        attributeAggregatorMap.Add(modifier.Type, aggregator);
                    }
                    // If this is a periodic effect, we don't add any attributes here. 
                    // They will be added as required on period expiry and stored in a separate structure
                    if (effectContext.Effect.Configs.PeriodConfig.Period <= 0) {
                        aggregator.AddAggregatorModifier(modifier.OperationType, modifier.Value);
                    }
                    // Recalculate new value by recomputing all aggregators
                    var aggregators = GetAggregatorsForAttribute(modifier.Type);
                    UpdateAttribute(aggregators, modifier.Type);
                }
            });
        }

        private async void CheckGameplayEffectForTimedEffects(EffectContext effectContext) 
        {
            await WaitForEffectExpiryTime(effectContext);
            var gameplayCues = effectContext.Effect.Configs.Cues;
            foreach (var cue in gameplayCues) {
                cue.HandleCue(effectContext.Target, CueEventMomentType.OnRemove);
            }
            // There could be multiple stacked effects, due to multiple casts
            // Remove one instance of this effect from the active list
            ModifyActiveGameplayEffect(effectContext, modifier => {
                
                modifiesAggregator.Remove(effectContext);
                // effectsModifyAggregator.RemoveEffect(effectContext);
                if (string.IsNullOrEmpty(modifier.Type)) return;

                // Find all remaining aggregators of the same type and recompute values
                var aggregators = GetAggregatorsForAttribute(modifier.Type);
                // If there are no aggregators, set base = current
                if (aggregators.Count() == 0) {
                    var current = target.GetBaseValue(modifier.Type);
                    if (current < 0) target.SetBaseValue(modifier.Type, 0f);
                    target.SetCurrentValue(modifier.Type, current);
                } else {
                    UpdateAttribute(aggregators, modifier.Type);
                }
            });
        }

        /// <summary>
        /// This function is used to do checks for things that may happen on a timed basis, such as
        /// periodic effects or effect expiry
        /// This is currently updated every frame.  Perhaps there are ways to make this more efficient?
        /// The main advantage of checking every frame is we can manipulate the WorldStartTime to
        /// effectively "refresh" the effect or end it at will.
        /// </summary>
        /// <param name="effectContext"></param>
        /// <returns> </returns>
        private async Task WaitForEffectExpiryTime(EffectContext effectContext) 
        {
            bool durationExpired = false;
            while (!durationExpired) {
                await UniTask.DelayFrame(0);

                if (effectContext.ForceRemoveEffect) {
                    durationExpired = true;
                } else if (effectContext.Effect.Configs.DurationConfig.Policy == DurationPolicy.Duration) {
                    // Check whether required time has expired
                    // We only need to do this for effects with a finite duration
                    durationExpired = effectContext.RemainingTime <= 0 ? true : false;
                } else if (effectContext.Effect.Configs.DurationConfig.Policy == DurationPolicy.Infinite) {
                    durationExpired = effectContext.StartTime <= 0 ? true : false;
                }
                // Periodic effects only occur if the period is > 0
                if (effectContext.Effect.Configs.PeriodConfig.Period > 0) {
                    CheckAndApplyPeriodicEffect(effectContext);
                }
                if (durationExpired) { // This effect is due for expiry
                    ApplyStackExpirationPolicy(effectContext, ref durationExpired);
                }
            }
        }

        private void CheckAndApplyPeriodicEffect(EffectContext effectContext) 
        {
            if (effectContext.TimeUntilNextPeriodApply <= 0) {
                // Apply gameplay effect defined for period.  
                if (effectContext.Effect.Configs.PeriodConfig.EffectOnExecute != null) {
                    effectContext.Instigator.ApplyEffectToTarget(
                        effectContext.SourceId, 
                        effectContext.Effect.Configs.PeriodConfig.EffectOnExecute, 
                        effectContext.Target
                    );
                }
                var gameplayCues = effectContext.Effect.Configs.Cues;
                foreach (var cue in gameplayCues) {
                    cue.HandleCue(effectContext.Target, CueEventMomentType.OnExecute);
                }
                effectContext.AddPeriodicEffectAttributeModifiers();
                effectContext.ResetPeriodicTime();
            }
        }

        private void ApplyStackExpirationPolicy(EffectContext effectContext, ref bool durationExpired) 
        {
            IEnumerable<EffectContext> matchingEffects;

            switch (effectContext.Effect.Configs.StackConfig.ExpirationPolicy) {
                case StackExpirationPolicy.ClearEntireStack: // Remove all effects which match
                    matchingEffects = GetMatchingStackedEffectsByEffect(effectContext);
                    if (matchingEffects == null) break;
                    foreach (var effect in matchingEffects) {
                        effect.EndEffect();
                    }
                    break;
                case StackExpirationPolicy.RemoveSingleStackAndRefreshDuration:
                    // Remove this effect, and reset all other durations to max
                    matchingEffects = GetMatchingStackedEffectsByEffect(effectContext);
                    if (matchingEffects == null) break;

                    foreach (var effect in matchingEffects) {
                        // We need to cater for the fact that the cooldown
                        // may have exceeded the actual limit by a little bit
                        // due to framerate.  So, when we reset the other cooldowns
                        // we need to account for this difference
                        var timeOverflow = effect.RemainingTime;
                        effect.ResetDuration(timeOverflow);
                    }
                    // This effect was going to expire anyway, but we put this here to be explicit to future code readers
                    effectContext.EndEffect();
                    break;
                case StackExpirationPolicy.RefreshDuration:
                    // Refreshing duration on expiry basically means the effect can never expire
                    matchingEffects = GetMatchingStackedEffectsByEffect(effectContext);
                    if (matchingEffects == null) break;
                    foreach (var effect in matchingEffects) {
                        effect.ResetDuration();
                        durationExpired = false; // Undo effect expiry.  This effect should never expire
                    }
                    break;
            }
        }


        private IEnumerable<EffectContext> GetMatchingStackedEffectsByEffect(EffectContext effectContext) 
        {
            IEnumerable<EffectContext> matchingStackedActiveEffects = null;
            switch (effectContext.Effect.Configs.StackConfig.Type) {
                // Stacking Type None:
                // Add effect as a separate instance. 
                case StackType.None:
                    break;
                case StackType.StackBySource:
                    matchingStackedActiveEffects = AllEffects.Where(x => x.Instigator == effectContext.Instigator && x.Effect == effectContext.Effect);
                    break;
                case StackType.StackByTarget:
                    matchingStackedActiveEffects = AllEffects.Where(x => x.Target == effectContext.Target && x.Effect == effectContext.Effect);
                    break;
            }
            return matchingStackedActiveEffects;
        }
    }
}

