using System.Linq;
using System.Collections;
using System.Collections.Generic;
using AbilitySystem.Interfaces;
using AbilitySystem;
using AbilitySystem.Attributes;
using UnityEngine;
using AbilitySystem.Cues;

namespace AbilitySystem.GameplayEffects {

    // 改变自己或别人的Attributes 和GameplayTags的途径
    [CreateAssetMenu(fileName = "Gameplay Effect", menuName = "Ability System/Gameplay Effect")]
    public class GameplayEffect : ScriptableObject {
        [SerializeField] private GameplayEffectPolicy gameplayEffectPolicy = new GameplayEffectPolicy();
        [SerializeField] private GameplayEffectTags gameplayEffectTags = new GameplayEffectTags();
        [SerializeField] private EffectPeriodicity periodicity = new EffectPeriodicity();
        [SerializeField] private List<GameplayCue> gameplayCues = new List<GameplayCue>();
        [SerializeField] private StackingPolicy stackingPolicy = new StackingPolicy();

        public GameplayEffectPolicy GameplayEffectPolicy => gameplayEffectPolicy;
        public GameplayEffectTags EffectTags => gameplayEffectTags;
        public EffectPeriodicity Periodicity => periodicity;
        public List<GameplayCue> GameplayCues => gameplayCues;
        public StackingPolicy StackingPolicy => stackingPolicy;
        public List<GameplayTag> GrantedTags => gameplayEffectTags.GrantedToASCTags.Added;
        // public IEnumerable<(GameplayTag Tag, GameplayEffect Effect)> GrantedEffectTags => GrantedTags.Select(x => (x, this));

        public bool ApplicationTagRequirementMet(AbilitySystemComponent ASC) {
            var requiredTagsPresent = true;
            var ignoredTagsAbsent = true;

            if (EffectTags.ApplyRequiredTags.RequirePresence.Count > 0) {
                requiredTagsPresent = ASC.ActiveTags.Any(x => EffectTags.ApplyRequiredTags.RequirePresence.Contains(x));
            }

            if (EffectTags.ApplyRequiredTags.RequireAbsence.Count > 0) {
                ignoredTagsAbsent = !ASC.ActiveTags.Any(x => EffectTags.ApplyRequiredTags.RequireAbsence.Contains(x));
            }


            return requiredTagsPresent && ignoredTagsAbsent;
        }

        public List<GameplayTag> GetOwningTags() {
            var tags = new List<GameplayTag>(gameplayEffectTags.GrantedToASCTags.Added.Count
                                            + gameplayEffectTags.EffectTags.Added.Count);

            tags.AddRange(gameplayEffectTags.GrantedToASCTags.Added);
            tags.AddRange(gameplayEffectTags.EffectTags.Added);
            return tags;
        }

        public Dictionary<AttributeType, Dictionary<ModifierOperationType, float>> CalculateModifierEffect(Dictionary<AttributeType, Dictionary<ModifierOperationType, float>> Existing = null) {
            
            Dictionary<AttributeType, Dictionary<ModifierOperationType, float>> modifierTotals;
            if (Existing == null) {
                modifierTotals = new Dictionary<AttributeType, Dictionary<ModifierOperationType, float>>();

            } else {
                modifierTotals = Existing;
            }

            foreach (var modifier in GameplayEffectPolicy.Modifiers) {
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

        public Dictionary<AttributeType, AttributeModificationValues> CalculateAttributeModification(
                    AbilitySystemComponent abilitySystem, 
                    Dictionary<AttributeType, Dictionary<ModifierOperationType, float>> modifiers, 
                    bool operateOnCurrentValue = false
        ) {
            var attributeModification = new Dictionary<AttributeType, AttributeModificationValues>();

            foreach (var attribute in modifiers) {
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
                    oldAttributeValue = abilitySystem.GetBaseValue(attribute.Key);
                } else {
                    oldAttributeValue = abilitySystem.GetCurrentValue(attribute.Key);
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

        public void ApplyInstantEffect(AbilitySystemComponent target) {
            // Modify base attribute values.  Collect the overall change for each modifier
            var modifierTotals = CalculateModifierEffect();
            var attributeModifications = CalculateAttributeModification(target, modifierTotals);

            // Finally, For each attribute, apply the new modified values
            foreach (var attribute in attributeModifications) {
                target.SetBaseValue(attribute.Key, attribute.Value.NewAttribueValue);

                // mark the corresponding aggregator as dirty so we can recalculate the current values
                var aggregators = target.ActiveEffectsContainer.ActiveEffectAttributeAggregator.GetAggregatorsForAttribute(attribute.Key);
                // Target.ActiveGameplayEffectsContainer.ActiveEffectAttributeAggregator.Select(x => x.Value[attribute.Key]).AttributeAggregatorMap.TryGetValue(attribute.Key, out var aggregator);
                if (aggregators.Count() != 0) {
                    target.ActiveEffectsContainer.UpdateAttribute(aggregators, attribute.Key);
                } else {
                    // No aggregators, so set current value = base value
                    target.SetCurrentValue(attribute.Key, target.GetBaseValue(attribute.Key));
                }
            }
        }

        // public bool ApplicationRequirementsPass(AbilitySystemComponent ASC) {
            // return _gameplayEffectTags.ApplicationTagRequirements.RequirePresence;
        // 
        //     return true;
        // }
    }
}

