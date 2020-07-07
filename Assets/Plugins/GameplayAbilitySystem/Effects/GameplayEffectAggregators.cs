using System.Linq;
using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Enums;
using UnityEngine.Events;
using GameplayAbilitySystem.Attributes;

namespace GameplayAbilitySystem.GameplayEffects {
    public class Aggregator {
        private AttributeType AttributeType;

        public Aggregator(AttributeType AttributeType) {
            this.AttributeType = AttributeType;
        }

        public Dictionary<ModifierOperationType, List<AggregatorModifier>> Mods { get; } = new Dictionary<ModifierOperationType, List<AggregatorModifier>>();
        public AggregatorEvent Dirtied { get; set; } = new AggregatorEvent();

        public void AddAggregatorMod(float EvaluatedMagnitude, ModifierOperationType ModifierOperation) {
            // If aggregator exists, check if we have a definition for this modifier operation
            if (!Mods.TryGetValue(ModifierOperation, out var aggregateMods)) {
                aggregateMods = new List<AggregatorModifier>();
                Mods.Add(ModifierOperation, aggregateMods);
            }
            aggregateMods.Add(new AggregatorModifier(EvaluatedMagnitude));
        }

        public void MarkDirty() {
            this.OnDirtied();
        }

        protected void OnDirtied() {
            Dirtied?.Invoke(this, this.AttributeType);
        }

        public float SumMods(List<AggregatorModifier> Mods) {
            return Mods.Sum(x => x.EvaluatedMagnitude);
        }

        public float ProductMods(List<AggregatorModifier> Mods) {
            return Mods.Select(x => x.EvaluatedMagnitude).Aggregate((result, item) => result * item);
        }

        public float GetAdditives() {
            var additive = 0f;
            if (Mods.TryGetValue(ModifierOperationType.Add, out var AddModifier)) {
                additive = SumMods(AddModifier);
            }

            return additive;
        }

        public float GetMultipliers() {
            var multiplier = 1f;
            var divider = 1f;
            if (Mods.TryGetValue(ModifierOperationType.Multiply, out var MultiplyModifiers)) {
                multiplier = ProductMods(Mods[ModifierOperationType.Multiply]);
            }

            if (Mods.TryGetValue(ModifierOperationType.Divide, out var DivideModifier)) {
                divider = ProductMods(Mods[ModifierOperationType.Divide]);
            }

            return multiplier / divider;
        }

        public float Evaluate(float BaseValue) {
            float additive = 0;
            float multiplicative = 1;
            float divisive = 1;

            if (Mods.TryGetValue(ModifierOperationType.Add, out var AddModifier)) {
                additive = SumMods(Mods[ModifierOperationType.Add]);
            }

            if (Mods.TryGetValue(ModifierOperationType.Multiply, out var MultiplyModifier)) {
                multiplicative = ProductMods(Mods[ModifierOperationType.Multiply]);
            }

            if (Mods.TryGetValue(ModifierOperationType.Divide, out var DivideModifier)) {
                divisive = ProductMods(Mods[ModifierOperationType.Divide]);
            }

            return (BaseValue + additive) * (multiplicative / divisive);
        }



    }

    public class AggregatorModifier {
        public AggregatorModifier(float EvaluatedMagnitude, float Stacks = 1) {
            this.Stacks = Stacks;
            this.EvaluatedMagnitude = EvaluatedMagnitude;
        }

        public float Stacks { get; private set; }
        public readonly float EvaluatedMagnitude;
    }

    public class AggregatorEvent : UnityEvent<Aggregator, AttributeType> {

    }

    public static partial class ExtensionMethods {
        public static float Evaluate(this IEnumerable<Aggregator> Aggregators, float BaseValue) {
            var additives = Aggregators.Select(x => x.GetAdditives()).Sum();
            var multipliers = Aggregators
                                    .Select(x => x.GetMultipliers())
                                    .Aggregate(1f, (result, item) => result * item);
            return (BaseValue + additives) * multipliers;
        }
    }

}

