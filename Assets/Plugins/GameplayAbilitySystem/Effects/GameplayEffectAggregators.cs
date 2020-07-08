using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using GAS.Attributes;

namespace GAS.GameplayEffects {
    public class Aggregator {
        private AttributeType attributeType;

        public Aggregator(AttributeType attributeType) {
            this.attributeType = attributeType;
        }

        private Dictionary<ModifierOperationType, List<AggregatorModifier>> modifiers
                                            = new Dictionary<ModifierOperationType, List<AggregatorModifier>>();
        public AggregatorEvent dirtied = new AggregatorEvent();

        public void AddAggregatorModifier(float evaluatedMagnitude, ModifierOperationType modifierOperation) {
            // If aggregator exists, check if we have a definition for this modifier operation
            if (!modifiers.TryGetValue(modifierOperation, out var aggregatorModifier)) {
                aggregatorModifier = new List<AggregatorModifier>();
                modifiers.Add(modifierOperation, aggregatorModifier);
            }
            aggregatorModifier.Add(new AggregatorModifier(evaluatedMagnitude));
        }

        public void MarkDirty() {
            this.OnDirtied();
        }

        protected void OnDirtied() {
            dirtied?.Invoke(this, this.attributeType);
        }

        public float SumMods(List<AggregatorModifier> Mods) {
            return Mods.Sum(x => x.EvaluatedMagnitude);
        }

        public float ProductMods(List<AggregatorModifier> Mods) {
            return Mods.Select(x => x.EvaluatedMagnitude).Aggregate((result, item) => result * item);
        }

        public float GetAdditives() {
            var additive = 0f;
            if (modifiers.TryGetValue(ModifierOperationType.Add, out var AddModifier)) {
                additive = SumMods(AddModifier);
            }

            return additive;
        }

        public float GetMultipliers() {
            var multiplier = 1f;
            var divider = 1f;
            if (modifiers.TryGetValue(ModifierOperationType.Multiply, out var MultiplyModifiers)) {
                multiplier = ProductMods(modifiers[ModifierOperationType.Multiply]);
            }

            if (modifiers.TryGetValue(ModifierOperationType.Divide, out var DivideModifier)) {
                divider = ProductMods(modifiers[ModifierOperationType.Divide]);
            }

            return multiplier / divider;
        }

        public float Evaluate(float BaseValue) {
            float additive = 0;
            float multiplicative = 1;
            float divisive = 1;

            if (modifiers.TryGetValue(ModifierOperationType.Add, out var AddModifier)) {
                additive = SumMods(modifiers[ModifierOperationType.Add]);
            }

            if (modifiers.TryGetValue(ModifierOperationType.Multiply, out var MultiplyModifier)) {
                multiplicative = ProductMods(modifiers[ModifierOperationType.Multiply]);
            }

            if (modifiers.TryGetValue(ModifierOperationType.Divide, out var DivideModifier)) {
                divisive = ProductMods(modifiers[ModifierOperationType.Divide]);
            }

            return (BaseValue + additive) * (multiplicative / divisive);
        }



    }

    public class AggregatorModifier {
        // 计算出的数量值
        public readonly float EvaluatedMagnitude;
        public float Stacks { get; private set; }

        public AggregatorModifier(float evaluatedMagnitude, float Stacks = 1) {
            this.Stacks = Stacks;
            this.EvaluatedMagnitude = evaluatedMagnitude;
        }

    }

    public class AggregatorEvent : UnityEvent<Aggregator, AttributeType> {

    }

    public static partial class ExtensionMethods {
        public static float Evaluate(this IEnumerable<Aggregator> aggregators, float baseValue) {
            var additives = aggregators.Select(x => x.GetAdditives()).Sum();
            var multipliers = aggregators
                                    .Select(x => x.GetMultipliers())
                                    .Aggregate(1f, (result, item) => result * item);
            return (baseValue + additives) * multipliers;
        }
    }

}

