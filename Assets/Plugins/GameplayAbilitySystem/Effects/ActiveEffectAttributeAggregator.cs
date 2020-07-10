using System.Linq;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes;
using UniRx.Async;

namespace GameplayAbilitySystem.Effects {

    // This is used to keep track of all the "temporary" attribute modifiers,
    // so we can calculate them all as f(Base, Added, Multiplied, Divided) = (Base + Added) * (Multiplied/Divided)
    public class ActiveEffectAttributeAggregator {
        private Dictionary<ActivedEffectData, Dictionary<AttributeType, Aggregator>> effectsAggregators = 
            new Dictionary<ActivedEffectData, Dictionary<AttributeType, Aggregator>>();

        public Dictionary<AttributeType, Aggregator> AddorGet(ActivedEffectData effectData) {
            if (!effectsAggregators.TryGetValue(effectData, out var attributeAggregators)) {
                attributeAggregators = new Dictionary<AttributeType, Aggregator>();
                effectsAggregators.Add(effectData, attributeAggregators);
            }
            return attributeAggregators;
        }

        public void RemoveEffect(ActivedEffectData EffectData) {
            this.effectsAggregators.Remove(EffectData);
        }

        public List<ActivedEffectData> GetAllActiveEffects() {
            return effectsAggregators.Keys.ToList();
        }

        public IEnumerable<Aggregator> GetAggregatorsForAttribute(AttributeType Attribute) {
            // Find all remaining aggregators of the same type and recompute values
            var aggregators = effectsAggregators
                                .Where(x => x.Value.ContainsKey(Attribute))
                                .Select(x => x.Value[Attribute]);

            var periodic = effectsAggregators
                            .Where(x => x.Key.Effect.Periodicity.Period > 0)
                            .Select(x => x.Key.GetPeriodicAggregatorForAttribute(Attribute))
                            .Where(x => x != null);

            return aggregators.Concat(periodic);
        }
    }
}

