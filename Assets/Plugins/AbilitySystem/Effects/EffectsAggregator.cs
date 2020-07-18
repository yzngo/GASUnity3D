using System.Linq;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes;
using Cysharp.Threading.Tasks;

namespace GameplayAbilitySystem.Effects 
{
    // This is used to keep track of all the "temporary" attribute modifiers,
    // so we can calculate them all as f(Base, Added, Multiplied, Divided) = (Base + Added) * (Multiplied/Divided)
    public class EffectsModifyAggregator 
    {
        private Dictionary<EffectContext, Dictionary<AttributeType, AttributeModifyAggregator>> effectAggregator = 
            new Dictionary<EffectContext, Dictionary<AttributeType, AttributeModifyAggregator>>();

        public Dictionary<AttributeType, AttributeModifyAggregator> AddorGet(EffectContext effectContext) 
        {
            if (!effectAggregator.TryGetValue(effectContext, out var attributeAggregators)) {
                attributeAggregators = new Dictionary<AttributeType, AttributeModifyAggregator>();
                effectAggregator.Add(effectContext, attributeAggregators);
            }
            return attributeAggregators;
        }

        public void RemoveEffect(EffectContext effectContext) 
        {
            effectAggregator.Remove(effectContext);
        }

        public List<EffectContext> GetAllEffects() 
        {
            return effectAggregator.Keys.ToList();
        }

        // 获取所有effect中,修改某个attribute的所有attributeAggregator
        public IEnumerable<AttributeModifyAggregator> GetAggregatorsForAttribute(AttributeType Attribute) 
        {
            // Find all remaining aggregators of the same type and recompute values
            var aggregators = effectAggregator
                                .Where(x => x.Value.ContainsKey(Attribute))
                                .Select(x => x.Value[Attribute]);
            var periodic = effectAggregator
                            .Where(x => x.Key.Effect.Configs.PeriodConfig.Period > 0)
                            .Select(x => x.Key.GetPeriodicAggregatorForAttribute(Attribute))
                            .Where(x => x != null);
            return aggregators.Concat(periodic);
        }
    }
}

