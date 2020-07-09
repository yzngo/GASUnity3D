using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameplayAbilitySystem.Interfaces;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Attributes;
using UnityEngine;
using GameplayAbilitySystem.Cues;

namespace GameplayAbilitySystem.Effects {

    using TotalModifies = Dictionary<AttributeType, Dictionary<ModifierOperationType, float>>;

    // 改变自己或别人的Attributes 和GameplayTags的途径
    [CreateAssetMenu(fileName = "Gameplay Effect", menuName = "Ability System/Gameplay Effect")]
    public class GameplayEffect : ScriptableObject {
        [SerializeField] private GameplayEffectPolicy gameplayEffectPolicy = new GameplayEffectPolicy();
        [SerializeField] private GameplayEffectTags gameplayEffectTags = new GameplayEffectTags();
        [SerializeField] private EffectPeriodicity periodicity = new EffectPeriodicity();
        [SerializeField] private List<GameplayCue> gameplayCues = new List<GameplayCue>();
        [SerializeField] private StackingPolicy stackingPolicy = new StackingPolicy();

        public GameplayEffectPolicy Policy => gameplayEffectPolicy;
        public GameplayEffectTags EffectTags => gameplayEffectTags;
        public EffectPeriodicity Periodicity => periodicity;
        public List<GameplayCue> GameplayCues => gameplayCues;
        public StackingPolicy StackingPolicy => stackingPolicy;
        public List<GameplayTag> GrantedTags => gameplayEffectTags.GrantedToASCTags.Added;

        public bool IsTagsRequiredMatch(AbilitySystem abilitySystem) {
            var requiredTagsPresent = true;
            var ignoredTagsAbsent = true;

            if (EffectTags.ApplyRequiredTags.RequirePresence.Count > 0) {
                requiredTagsPresent = abilitySystem.ActiveTags.Any(x => EffectTags.ApplyRequiredTags.RequirePresence.Contains(x));
            }

            if (EffectTags.ApplyRequiredTags.RequireAbsence.Count > 0) {
                ignoredTagsAbsent = !abilitySystem.ActiveTags.Any(x => EffectTags.ApplyRequiredTags.RequireAbsence.Contains(x));
            }


            return requiredTagsPresent && ignoredTagsAbsent;
        }

        // public List<GameplayTag> GetOwningTags() {
        //     var tags = new List<GameplayTag>(gameplayEffectTags.GrantedToASCTags.Added.Count
        //                                     + gameplayEffectTags.EffectTags.Added.Count);

        //     tags.AddRange(gameplayEffectTags.GrantedToASCTags.Added);
        //     tags.AddRange(gameplayEffectTags.EffectTags.Added);
        //     return tags;
        // }

        // 求[此effect]聚合之后的对所有属性的所有操作集合
        // e.g.     HP ->  Add 100, Multi 0.5, Div 0.1
        //          MP ->  Add 10,  Multi 0,   Div 0
        public TotalModifies CalculateModifiers() 
        {
            var totalModifies = new TotalModifies();

            foreach (var modifier in Policy.Modifiers) {
                // 当前attributeType的条目是否存在
                if (!totalModifies.TryGetValue(modifier.AttributeType, out var typeModifiers)) {
                    typeModifiers = new Dictionary<ModifierOperationType, float>();
                    totalModifies.Add(modifier.AttributeType, typeModifiers);
                }
                // 当前attribute的operation条目是否存在
                if (!typeModifiers.TryGetValue(modifier.ModifierOperation, out var value)) {
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
                    typeModifiers.Add(modifier.ModifierOperation, value);
                }

                switch (modifier.ModifierOperation) {
                    case ModifierOperationType.Add:
                        totalModifies[modifier.AttributeType][modifier.ModifierOperation] += modifier.ScaledMagnitude;
                        break;
                    case ModifierOperationType.Multiply:
                        totalModifies[modifier.AttributeType][modifier.ModifierOperation] *= modifier.ScaledMagnitude;
                        break;
                    case ModifierOperationType.Divide:
                        totalModifies[modifier.AttributeType][modifier.ModifierOperation] *= modifier.ScaledMagnitude;
                        break;
                }
            }
            return totalModifies;
        }

        // e.g. HP -> oldValue 100 newValue 200
        //      MP -> oldValue 200 newValue 189
        public Dictionary<AttributeType, ModifyArrtibuteValues> CalculateAttributes(
                            AbilitySystem abilitySystem, TotalModifies totalModifies, bool operateOnCurrentValue = false
        ) {
            var totalCaculatedAttributes = new Dictionary<AttributeType, ModifyArrtibuteValues>();

            foreach (var modifyByType in totalModifies) {
                if (!modifyByType.Value.TryGetValue(ModifierOperationType.Add, out var addition)) {
                    addition = 0;
                }
                if (!modifyByType.Value.TryGetValue(ModifierOperationType.Multiply, out var multiplication)) {
                    multiplication = 1;
                }
                if (!modifyByType.Value.TryGetValue(ModifierOperationType.Divide, out var division)) {
                    division = 1;
                }

                var oldValue = 0f;
                if (!operateOnCurrentValue) {
                    oldValue = abilitySystem.GetBaseValue(modifyByType.Key);
                } else {
                    oldValue = abilitySystem.GetCurrentValue(modifyByType.Key);
                }

                var newValue = (oldValue + addition) * (multiplication / division);

                if (!totalCaculatedAttributes.TryGetValue(modifyByType.Key, out ModifyArrtibuteValues values)) {
                    values = new ModifyArrtibuteValues();
                    totalCaculatedAttributes.Add(modifyByType.Key, values);
                }
                values.newValue += newValue;
                values.oldValue += oldValue;
            }
            return totalCaculatedAttributes;
        }

        public void ApplyInstantEffect(AbilitySystem target) {
            // Modify base attribute values.  Collect the overall change for each modifier
            var modifierTotals = CalculateModifiers();
            var attributeModifications = CalculateAttributes(target, modifierTotals);

            // Finally, For each attribute, apply the new modified values
            foreach (var attribute in attributeModifications) {
                target.SetBaseValue(attribute.Key, attribute.Value.newValue);

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

        // public bool ApplicationRequirementsPass(AbilitySystemComponent abilitySystem) {
            // return _gameplayEffectTags.ApplicationTagRequirements.RequirePresence;
        // 
        //     return true;
        // }
    }
}

