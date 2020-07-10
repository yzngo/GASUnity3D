using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Attributes;
using UnityEngine;
using GameplayAbilitySystem.Cues;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem.Effects 
{

    using TotalModifies = Dictionary<AttributeType, Dictionary<ModifierOperationType, float>>;

    // 改变自己或别人的Attributes 和GameplayTags的途径
    [CreateAssetMenu(fileName = "Gameplay Effect", menuName = "Ability System/Gameplay Effect")]
    public class Effect : ScriptableObject 
    {
        [SerializeField] private EffectPolicy gameplayEffectPolicy = new EffectPolicy();
        [SerializeField] private PeriodConfig periodicity = new PeriodConfig();
        [SerializeField] private StackConfig stackPolicy = new StackConfig();
        [SerializeField] private EffectTagContainer gameplayEffectTags = new EffectTagContainer();
        [SerializeField] private List<GameplayCue> gameplayCues = new List<GameplayCue>();

        public EffectPolicy Policy => gameplayEffectPolicy;
        public EffectTagContainer EffectTags => gameplayEffectTags;
        public PeriodConfig PeriodPolicy => periodicity;
        public List<GameplayCue> GameplayCues => gameplayCues;
        public StackConfig StackPolicy => stackPolicy;
        public List<GameplayTag> GrantedTags => gameplayEffectTags.GrantedToInstigatorTags;

        public bool IsTagsRequiredMatch(AbilitySystem target) {
            var requiredTagsPresent = true;
            var ignoredTagsAbsent = true;

            // if (EffectTags.ApplyRequiredTags.RequirePresence.Count > 0) {
            //     requiredTagsPresent = target.ActiveTags.Any(x => EffectTags.ApplyRequiredTags.RequirePresence.Contains(x));
            // }

            // if (EffectTags.ApplyRequiredTags.RequireAbsence.Count > 0) {
            //     ignoredTagsAbsent = !target.ActiveTags.Any(x => EffectTags.ApplyRequiredTags.RequireAbsence.Contains(x));
            // }
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
                            AbilitySystem target, TotalModifies totalModifies, bool operateOnCurrentValue = false
        ) {
            var totalAttributeChange = new Dictionary<AttributeType, ModifyArrtibuteValues>();

            foreach (var changeByType in totalModifies) {
                if (!changeByType.Value.TryGetValue(ModifierOperationType.Add, out var addition)) {
                    addition = 0;
                }
                if (!changeByType.Value.TryGetValue(ModifierOperationType.Multiply, out var multiplication)) {
                    multiplication = 1;
                }
                if (!changeByType.Value.TryGetValue(ModifierOperationType.Divide, out var division)) {
                    division = 1;
                }

                float oldValue = 0f;
                if (!operateOnCurrentValue) {
                    oldValue = target.GetBaseValue(changeByType.Key);
                } else {
                    oldValue = target.GetCurrentValue(changeByType.Key);
                }
                float newValue = (oldValue + addition) * (multiplication / division);

                if (!totalAttributeChange.TryGetValue(changeByType.Key, out ModifyArrtibuteValues values)) {
                    values = new ModifyArrtibuteValues();
                    totalAttributeChange.Add(changeByType.Key, values);
                }
                values.newValue += newValue;
                values.oldValue += oldValue;
            }
            return totalAttributeChange;
        }

        // instant 一定改变base值
        public void ApplyInstantEffect(AbilitySystem target) {
            var totalModifies = CalculateModifiers();
            var totalAttributeChange = CalculateAttributes(target, totalModifies);

            // For each attribute, apply the new modified values
            foreach (var changeByType in totalAttributeChange) {
                target.SetBaseValue(changeByType.Key, changeByType.Value.newValue);

                // mark the corresponding aggregator as dirty so we can recalculate the current values
                var aggregators = target.EffectsContainer.effectsModifyAggregator.GetAggregatorsForAttribute(changeByType.Key);
                if (aggregators.Count() != 0) {
                    target.EffectsContainer.UpdateAttribute(aggregators, changeByType.Key);
                } else {
                    // No aggregators, so set current value = base value
                    target.SetCurrentValue(changeByType.Key, target.GetBaseValue(changeByType.Key));
                }
            }
        }

        public class ModifyArrtibuteValues {
            public float oldValue = 0f;
            public float newValue = 0f;
        }

    }
}

