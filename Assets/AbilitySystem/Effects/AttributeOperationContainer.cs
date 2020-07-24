using System.Linq;
using System.Collections.Generic;

namespace GameplayAbilitySystem
{
    public class AttributeOperationContainer 
    {
        private Dictionary<OperationType, List<float>> operationContainer = new Dictionary<OperationType, List<float>>();

        public void AddOperation(OperationType operation, float value) 
        {
            if (!operationContainer.TryGetValue(operation, out List<float> values)) {
                values = new List<float>();
                operationContainer.Add(operation, values);
            }
            values.Add(value);
        }

        public float GetAdditives() 
        {
            float additive = 0f;
            if (operationContainer.TryGetValue(OperationType.Add, out List<float> values)) {
                additive = SumValues(values);
            }
            return additive;
        }

        public float GetMultipliers() 
        {
            float multiplier = 1f;
            float divider = 1f;

            List<float> values;
            if (operationContainer.TryGetValue(OperationType.Multiply, out values)) {
                multiplier = ProductValues(values);
            }
            if (operationContainer.TryGetValue(OperationType.Divide, out values)) {
                divider = ProductValues(values);
            }
            return multiplier / divider;
        }

        public float Evaluate(float baseValue) 
        {
            float additive = 0;
            float multiplicative = 1;
            float divisive = 1;

            List<float> values;
            if (operationContainer.TryGetValue(OperationType.Add, out values)) {
                additive = SumValues(values);
            }
            if (operationContainer.TryGetValue(OperationType.Multiply, out values)) {
                multiplicative = ProductValues(values);
            }
            if (operationContainer.TryGetValue(OperationType.Divide, out values)) {
                divisive = ProductValues(values);
            }
            return (baseValue + additive) * (multiplicative / divisive);
        }

        private float SumValues(List<float> values) => values.Sum(x => x);
        private float ProductValues(List<float> values) => values.Select(x => x).Aggregate((result, item) => result * item);
    }

    public static partial class ExtensionMethods 
    {
        public static float Evaluate(this IEnumerable<AttributeOperationContainer> operationContainers, float valueToEvaluate) 
        {
            float additives = operationContainers.Select(x => x.GetAdditives()).Sum();
            float multipliers = operationContainers.Select(x => x.GetMultipliers()).Aggregate(1f, (result, item) => result * item);
            return (valueToEvaluate + additives) * multipliers;
        }
    }
}