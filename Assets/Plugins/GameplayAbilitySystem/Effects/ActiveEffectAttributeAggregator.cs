using System.Linq;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes;
using UniRx.Async;

namespace GameplayAbilitySystem.GameplayEffects {

    // 激活的effect的属性聚合器
    // This is used to keep track of all the "temporary" attribute modifiers,
    // so we can calculate them all as f(Base, Added, Multiplied, Divided) = (Base + Added) * (Multiplied/Divided)
    public class ActiveEffectAttributeAggregator {
        private Dictionary<ActiveGameplayEffectData, Dictionary<AttributeType, Aggregator>> Map = 
            new Dictionary<ActiveGameplayEffectData, Dictionary<AttributeType, Aggregator>>();


        public Dictionary<AttributeType, Aggregator> AddorGet(ActiveGameplayEffectData EffectData) {
            if (!Map.TryGetValue(EffectData, out var attributeAggregatorMap)) {
                attributeAggregatorMap = new Dictionary<AttributeType, Aggregator>();
                Map.Add(EffectData, attributeAggregatorMap);
            }
            return attributeAggregatorMap;
        }

        public void RemoveEffect(ActiveGameplayEffectData EffectData) {
            this.Map.Remove(EffectData);
        }

        public List<ActiveGameplayEffectData> GetAllActiveEffects() {
            return Map.Keys.ToList();
        }

        public IEnumerable<Aggregator> GetAggregatorsForAttribute(AttributeType Attribute) {
            // Find all remaining aggregators of the same type and recompute values
            var aggregators = Map
                                .Where(x => x.Value.ContainsKey(Attribute))
                                .Select(x => x.Value[Attribute]);

            var periodic = Map
                            .Where(x => x.Key.Effect.Periodicity.Period > 0)
                            .Select(x => x.Key.GetPeriodicAggregatorForAttribute(Attribute))
                            .Where(x => x != null);

            return aggregators.Concat(periodic);
        }
    }
}

