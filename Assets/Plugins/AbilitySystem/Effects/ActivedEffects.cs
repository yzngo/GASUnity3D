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
        private Dictionary<EffectContext, Dictionary<string, AttributeOperationContainer>> effects = 
                                        new Dictionary<EffectContext, Dictionary<string, AttributeOperationContainer>>();

        public List<EffectContext> AllEffects => effects.Keys.ToList();

        public ActivedEffects(AbilitySystem target) 
        {
            this.target = target;
        }

        public async void ApplyDurationalEffect(EffectContext effectContext) 
        {
            int stacks = GetStackedEffectsSameAs(effectContext)?.Count() ?? -1;
            int maxStacks = effectContext.Effect.Configs.StackConfig.MaxStacks;

            if (stacks < maxStacks) {
                ApplyModifier(effectContext);
                if (effectContext.Effect.Configs.DurationConfig.Policy != DurationPolicy.Instant) {
                    await WaitForExpiredOf(effectContext);
                }
            }
        }

        public void UpdateAttribute(string attributeType, IEnumerable<AttributeOperationContainer> operations) 
        {
            float baseValue = target.GetBaseValue(attributeType);
            float newCurrentValue = operations.Evaluate(baseValue);
            target.SetCurrentValue(attributeType, newCurrentValue);
        }

        public IEnumerable<AttributeOperationContainer> GetAllOperationTo(string attributeType) 
        {
            var operation = effects
                                .Where(x => x.Value.ContainsKey(attributeType))
                                .Select(x => x.Value[attributeType]);
            var periodic = effects
                            .Where(x => x.Key.Effect.Configs.PeriodConfig.Period > 0)
                            .Select(x => x.Key.GetPeriodicOperationsFor(attributeType))
                            .Where(x => x != null);
            return operation.Concat(periodic);
        }

        private void ApplyModifier(EffectContext effectContext)
        {
            Dictionary<string, AttributeOperationContainer> operations;
            if (!effects.TryGetValue(effectContext, out operations)) {
                operations = new Dictionary<string, AttributeOperationContainer>();
                effects.Add(effectContext, operations);
            }

            foreach(ModifierConfig modifier in effectContext.Effect.Configs.Modifiers) {
                if (!string.IsNullOrEmpty(modifier.AttributeType)) {

                    if (!operations.TryGetValue(modifier.AttributeType, out AttributeOperationContainer operation)) {
                        operation = new AttributeOperationContainer();
                        operations.Add(modifier.AttributeType, operation);
                    }
                    // If this is a periodic effect, we don't add any attributes here. 
                    // They will be added as required on period expiry and stored in a separate structure
                    if (effectContext.Effect.Configs.PeriodConfig.Period <= 0) {
                        operation.AddOperation(modifier.OperationType, modifier.Value);
                    }
                    target.ReEvaluateCurrentValueFor(modifier.AttributeType);
                    // Recalculate new value by recomputing all aggregators
                }
            }
        }

        private void ModifyActiveEffect(EffectContext effectContext, Action<ModifierConfig> action) 
        {
            foreach (var modifier in effectContext.Effect.Configs.Modifiers) {
                action(modifier);
            }
            // If there are no gameplay effect modifiers, we need to add or get an empty entry
            if (effectContext.Effect.Configs.Modifiers.Count == 0) {
                action((new ModifierConfig()));
            }
        }

        // 处理effect应用之后的逻辑
        private async Task WaitForExpiredOf(EffectContext effectContext) 
        {
            bool expired = false;
            while (!expired) {
                await UniTask.DelayFrame(0);

                if (effectContext.ForceRemoveEffect) {
                    expired = true;
                } else if (effectContext.Effect.Configs.DurationConfig.Policy == DurationPolicy.Duration) {
                    expired = effectContext.RemainingDuration <= 0 ? true : false;
                } else if (effectContext.Effect.Configs.DurationConfig.Policy == DurationPolicy.Infinite) {
                    expired = effectContext.StartTime <= 0 ? true : false;
                }

                if (effectContext.Effect.Configs.PeriodConfig.Period > 0) {
                    if (effectContext.PeriodicRemainingDuration <= 0) {
                        ApplyPeriodicEffect(effectContext);
                    }
                }
                if (expired) {
                    ApplyStackExpirationPolicy(effectContext, ref expired);
                }
            }

            List<EffectCues> cues = effectContext.Effect.Configs.Cues;
            foreach (var cue in cues) {
                cue.HandleCue(effectContext.Target, CueEventMomentType.OnRemove);
            }
            // There could be multiple stacked effects, due to multiple casts
            // Remove one instance of this effect from the active list
            ModifyActiveEffect(effectContext, modifier => {
                
                effects.Remove(effectContext);
                // effectsModifyAggregator.RemoveEffect(effectContext);
                if (string.IsNullOrEmpty(modifier.AttributeType)) return;

                // Find all remaining aggregators of the same type and recompute values
                var aggregators = GetAllOperationTo(modifier.AttributeType);
                // If there are no aggregators, set base = current
                if (aggregators.Count() == 0) {
                    var current = target.GetBaseValue(modifier.AttributeType);
                    if (current < 0) target.SetBaseValue(modifier.AttributeType, 0f);
                    target.SetCurrentValue(modifier.AttributeType, current);
                } else {
                    UpdateAttribute(modifier.AttributeType, aggregators);
                }
            });
        }

        private void ApplyPeriodicEffect(EffectContext effectContext) 
        {
            if (effectContext.Effect.Configs.PeriodConfig.EffectOnExecute != null) {
                effectContext.Instigator.ApplyEffectToTarget(
                    effectContext.SourceId, 
                    effectContext.Effect.Configs.PeriodConfig.EffectOnExecute, 
                    effectContext.Target
                );
            }
            List<EffectCues> cues = effectContext.Effect.Configs.Cues;
            foreach (var cue in cues) {
                cue.HandleCue(effectContext.Target, CueEventMomentType.OnExecute);
            }
            // apply 为什么要add到列表?
            effectContext.ApplyPeriodicOperations();
            effectContext.ResetPeriodicTime();
        }

        private void ApplyStackExpirationPolicy(EffectContext effectContext, ref bool expired) 
        {
            IEnumerable<EffectContext> effects = GetStackedEffectsSameAs(effectContext);
            if (effects == null) {
                return;
            }

            switch (effectContext.Effect.Configs.StackConfig.ExpirationPolicy) {
                case StackExpirationPolicy.ClearEntireStack:
                    foreach (var effect in effects) {
                        effect.EndEffect();
                    }
                    break;
                case StackExpirationPolicy.RemoveSingleStackAndRefreshDuration:

                    foreach (var effect in effects) {
                        effect.ResetStartTime();
                    }
                    effectContext.EndEffect();
                    break;
                case StackExpirationPolicy.RefreshDuration:
                    // Refreshing duration on expiry basically means the effect can never expire
                    // Undo effect expiry.  This effect should never expire
                    foreach (var effect in effects) {
                        effect.ResetStartTime();
                        expired = false; 
                    }
                    break;
            }
        }

        private IEnumerable<EffectContext> GetStackedEffectsSameAs(EffectContext effectContext) 
        {
            IEnumerable<EffectContext> effects = null;
            switch (effectContext.Effect.Configs.StackConfig.Type) {
                case StackType.None:
                    break;
                case StackType.StackBySource:
                    effects = AllEffects.Where(x => x.Instigator == effectContext.Instigator && x.Effect == effectContext.Effect);
                    break;
                case StackType.StackByTarget:
                    effects = AllEffects.Where(x => x.Target == effectContext.Target && x.Effect == effectContext.Effect);
                    break;
            }
            return effects;
        }
    }
}

