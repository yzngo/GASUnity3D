using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GAS.Interfaces;
using GAS.Statics;
using GAS;
using GAS.Attributes;
using GAS.Enums;
using UnityEngine;
using GAS.GameplayCues;

namespace GAS.GameplayEffects {

    // 改变自己或别人的Attributes 和GameplayTags的途径
    [CreateAssetMenu(fileName = "Gameplay Effect", menuName = "Ability System/Gameplay Effect")]
    public class GameplayEffect : ScriptableObject {
        [SerializeField] GameplayEffectPolicy gameplayEffectPolicy = new GameplayEffectPolicy();

        [SerializeField] GameplayEffectTags gameplayEffectTags = new GameplayEffectTags();

        public EffectPeriodicity Period;

        [SerializeField] public List<GameplayCue> GameplayCues = new List<GameplayCue>();

        public StackingPolicy StackingPolicy = new StackingPolicy();
        public GameplayEffectTags GameplayEffectTags { get => gameplayEffectTags; }
        public GameplayEffectPolicy GameplayEffectPolicy { get => gameplayEffectPolicy; }

        public IEnumerable<(GameplayTag Tag, GameplayEffect Effect)> GrantedEffectTags => this.GrantedTags.Select(x => (x, this));

        public bool ApplicationTagRequirementMet(IGameplayAbilitySystem AbilitySystem) {
            var requiredTagsPresent = true;
            var ignoredTagsAbsent = true;

            if (this.GameplayEffectTags.ApplicationTagRequirements.RequirePresence.Count > 0) {
                requiredTagsPresent = AbilitySystem.ActiveTags.Any(x => this.GameplayEffectTags.ApplicationTagRequirements.RequirePresence.Contains(x));
            }

            if (this.GameplayEffectTags.ApplicationTagRequirements.RequireAbsence.Count > 0) {
                ignoredTagsAbsent = !AbilitySystem.ActiveTags.Any(x => this.GameplayEffectTags.ApplicationTagRequirements.RequireAbsence.Contains(x));
            }


            return requiredTagsPresent && ignoredTagsAbsent;
        }

        public List<GameplayTag> GetOwningTags() {
            var tags = new List<GameplayTag>(gameplayEffectTags.GrantedTags.Added.Count
                                            + gameplayEffectTags.AssetTags.Added.Count);

            tags.AddRange(gameplayEffectTags.GrantedTags.Added);
            tags.AddRange(gameplayEffectTags.AssetTags.Added);

            return tags;
        }

        public List<GameplayTag> GrantedTags => gameplayEffectTags.GrantedTags.Added;

        public bool ApplicationRequirementsPass(AbilitySystemComponent AbilitySystem) {
            // return _gameplayEffectTags.ApplicationTagRequirements.RequirePresence;

            return true;
        }

        public Dictionary<AttributeType, Dictionary<ModifierOperationType, float>> CalculateModifierEffect(Dictionary<AttributeType, Dictionary<ModifierOperationType, float>> Existing = null) {
            Dictionary<AttributeType, Dictionary<ModifierOperationType, float>> modifierTotals;
            if (Existing == null) {
                modifierTotals = new Dictionary<AttributeType, Dictionary<ModifierOperationType, float>>();

            } else {
                modifierTotals = Existing;
            }

            foreach (var modifier in this.GameplayEffectPolicy.Modifiers) {
                if (!modifierTotals.TryGetValue(modifier.Attribute, out var modifierType)) {
                    // This attribute hasn't been recorded before, so create a blank new record
                    modifierType = new Dictionary<ModifierOperationType, float>();
                    modifierTotals.Add(modifier.Attribute, modifierType);
                }

                if (!modifierType.TryGetValue(modifier.ModifierOperation, out var value)) {
                    value = 0;
                    switch (modifier.ModifierOperation) {
                        case ModifierOperationType.Multiply:
                            value = 1;
                            break;
                        case ModifierOperationType.Divide:
                            value = 1;
                            break;
                        default:
                            value = 0;
                            break;
                    }
                    modifierType.Add(modifier.ModifierOperation, value);

                }

                switch (modifier.ModifierOperation) {
                    case ModifierOperationType.Add:
                        modifierTotals[modifier.Attribute][modifier.ModifierOperation] += modifier.ScaledMagnitude;
                        break;
                    case ModifierOperationType.Multiply:
                        modifierTotals[modifier.Attribute][modifier.ModifierOperation] *= modifier.ScaledMagnitude;
                        break;
                    case ModifierOperationType.Divide:
                        modifierTotals[modifier.Attribute][modifier.ModifierOperation] *= modifier.ScaledMagnitude;
                        break;
                }
            }

            return modifierTotals;
        }

        public Dictionary<AttributeType, AttributeModificationValues> CalculateAttributeModification(IGameplayAbilitySystem AbilitySystem, Dictionary<AttributeType, Dictionary<ModifierOperationType, float>> Modifiers, bool operateOnCurrentValue = false) {
            var attributeModification = new Dictionary<AttributeType, AttributeModificationValues>();

            foreach (var attribute in Modifiers) {
                if (!attribute.Value.TryGetValue(ModifierOperationType.Add, out var addition)) {
                    addition = 0;
                }

                if (!attribute.Value.TryGetValue(ModifierOperationType.Multiply, out var multiplication)) {
                    multiplication = 1;
                }

                if (!attribute.Value.TryGetValue(ModifierOperationType.Divide, out var division)) {
                    division = 1;
                }

                var oldAttributeValue = 0f;
                if (!operateOnCurrentValue) {
                    oldAttributeValue = AbilitySystem.GetNumericAttributeBase(attribute.Key);
                } else {
                    oldAttributeValue = AbilitySystem.GetNumericAttributeCurrent(attribute.Key);
                }

                var newAttributeValue = (oldAttributeValue + addition) * (multiplication / division);

                if (!attributeModification.TryGetValue(attribute.Key, out var values)) {
                    values = new AttributeModificationValues();
                    attributeModification.Add(attribute.Key, values);
                }

                values.NewAttribueValue += newAttributeValue;
                values.OldAttributeValue += oldAttributeValue;

            }

            return attributeModification;
        }

        public void ApplyInstantEffect(IGameplayAbilitySystem Target) {
            // Modify base attribute values.  Collect the overall change for each modifier
            var modifierTotals = this.CalculateModifierEffect();
            var attributeModifications = this.CalculateAttributeModification(Target, modifierTotals);

            // Finally, For each attribute, apply the new modified values
            foreach (var attribute in attributeModifications) {
                Target.SetNumericAttributeBase(attribute.Key, attribute.Value.NewAttribueValue);

                // mark the corresponding aggregator as dirty so we can recalculate the current values
                var aggregators = Target.ActiveGameplayEffectsContainer.ActiveEffectAttributeAggregator.GetAggregatorsForAttribute(attribute.Key);
                // Target.ActiveGameplayEffectsContainer.ActiveEffectAttributeAggregator.Select(x => x.Value[attribute.Key]).AttributeAggregatorMap.TryGetValue(attribute.Key, out var aggregator);
                if (aggregators.Count() != 0) {
                    Target.ActiveGameplayEffectsContainer.UpdateAttribute(aggregators, attribute.Key);
                } else {
                    // No aggregators, so set current value = base value
                    Target.SetNumericAttributeCurrent(attribute.Key, Target.GetNumericAttributeBase(attribute.Key));
                }
            }
        }

    }



}

