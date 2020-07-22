using System.Linq;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes;

namespace GameplayAbilitySystem.Effects 
{
    public class AttributeOperationContainer 
    {
        // e.g. HP
        //      add     10 40 50
        //      multi   0.1 0.3 
        private Dictionary<OperationType, List<float>> operationContainer = new Dictionary<OperationType, List<float>>();

        public void Add(OperationType operation, float value) 
        {
            if (!operationContainer.TryGetValue(operation, out List<float> values)) {
                values = new List<float>();
                operationContainer.Add(operation, values);
            }
            values.Add(value);
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
            if (operationContainer.TryGetValue(OperationType.Add, out var modifyValues)) {
                additive = SumModifier(modifyValues);
            }
            return additive;
        }

        public float GetMultipliers() 
        {
            var multiplier = 1f;
            var divider = 1f;
            List<float> modifyValues;
            if (operationContainer.TryGetValue(OperationType.Multiply, out modifyValues)) {
                multiplier = ProductModifier(operationContainer[OperationType.Multiply]);
            }
            if (operationContainer.TryGetValue(OperationType.Divide, out modifyValues)) {
                divider = ProductModifier(operationContainer[OperationType.Divide]);
            }
            return multiplier / divider;
        }

        public float Evaluate(float baseValue) 
        {
            float additive = 0;
            float multiplicative = 1;
            float divisive = 1;

            if (operationContainer.TryGetValue(OperationType.Add, out var modifyValues)) {
                additive = SumModifier(operationContainer[OperationType.Add]);
            }
            if (operationContainer.TryGetValue(OperationType.Multiply, out modifyValues)) {
                multiplicative = ProductModifier(operationContainer[OperationType.Multiply]);
            }
            if (operationContainer.TryGetValue(OperationType.Divide, out modifyValues)) {
                divisive = ProductModifier(operationContainer[OperationType.Divide]);
            }
            return (baseValue + additive) * (multiplicative / divisive);
        }
    }

    public static partial class ExtensionMethods 
    {
        public static float Evaluate(this IEnumerable<AttributeOperationContainer> aggregators, float baseValue) 
        {
            var additives = aggregators.Select(x => x.GetAdditives()).Sum();
            var multipliers = aggregators
                                    .Select(x => x.GetMultipliers())
                                    .Aggregate(1f, (result, item) => result * item);
            return (baseValue + additives) * multipliers;
        }
    }
}