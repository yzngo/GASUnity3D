using System.Linq;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes;

namespace GameplayAbilitySystem.Effects 
{
    public class AttributeModifyAggregator 
    {
        // e.g. HP
        //      add     10 40 50
        //      multi   0.1 0.3 
        private AttributeType attributeType;
        private Dictionary<ModifierOperationType, List<float>> modifiers = new Dictionary<ModifierOperationType, List<float>>();

        public AttributeModifyAggregator(AttributeType attributeType) 
        {
            this.attributeType = attributeType;
        }

        public void AddAggregatorModifier(ModifierOperationType modifierOperation, float evaluatedValue) 
        {
            if (!modifiers.TryGetValue(modifierOperation, out var modifyValues)) {
                modifyValues = new List<float>();
                modifiers.Add(modifierOperation, modifyValues);
            }
            modifyValues.Add(evaluatedValue);
        }

        private float SumModifier(List<float> modifyValues) 
        {
            return modifyValues.Sum(x => x);
        }

        public float ProductModifier(List<float> modifyValues) 
        {
            return modifyValues.Select(x => x).Aggregate((result, item) => result * item);
        }

        public float GetAdditives() 
        {
            var additive = 0f;
            if (modifiers.TryGetValue(ModifierOperationType.Add, out var modifyValues)) {
                additive = SumModifier(modifyValues);
            }
            return additive;
        }

        public float GetMultipliers() 
        {
            var multiplier = 1f;
            var divider = 1f;
            List<float> modifyValues;
            if (modifiers.TryGetValue(ModifierOperationType.Multiply, out modifyValues)) {
                multiplier = ProductModifier(modifiers[ModifierOperationType.Multiply]);
            }
            if (modifiers.TryGetValue(ModifierOperationType.Divide, out modifyValues)) {
                divider = ProductModifier(modifiers[ModifierOperationType.Divide]);
            }
            return multiplier / divider;
        }

        public float Evaluate(float baseValue) 
        {
            float additive = 0;
            float multiplicative = 1;
            float divisive = 1;

            if (modifiers.TryGetValue(ModifierOperationType.Add, out var modifyValues)) {
                additive = SumModifier(modifiers[ModifierOperationType.Add]);
            }
            if (modifiers.TryGetValue(ModifierOperationType.Multiply, out modifyValues)) {
                multiplicative = ProductModifier(modifiers[ModifierOperationType.Multiply]);
            }
            if (modifiers.TryGetValue(ModifierOperationType.Divide, out modifyValues)) {
                divisive = ProductModifier(modifiers[ModifierOperationType.Divide]);
            }
            return (baseValue + additive) * (multiplicative / divisive);
        }
    }

    public static partial class ExtensionMethods 
    {
        public static float Evaluate(this IEnumerable<AttributeModifyAggregator> aggregators, float baseValue) 
        {
            var additives = aggregators.Select(x => x.GetAdditives()).Sum();
            var multipliers = aggregators
                                    .Select(x => x.GetMultipliers())
                                    .Aggregate(1f, (result, item) => result * item);
            return (baseValue + additives) * multipliers;
        }
    }
}