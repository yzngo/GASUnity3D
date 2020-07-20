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
    [CreateAssetMenu(fileName = "Gameplay Effect", menuName = "Ability System/Gameplay Effect")]
    public class Effect : ScriptableObject 
    {
        [SerializeField] private int id = default;
        [SerializeField] private Sprite icon = default;
        [SerializeField] private EffectConfigs configs = default;

        public int Id => id;
        public Sprite Icon => icon;
        public EffectConfigs Configs => configs;



        // 求[此effect]聚合之后的对所有属性的所有操作集合
        // e.g.     HP ->  Add 100, Multi 0.5, Div 0.1
        //          MP ->  Add 10,  Multi 0,   Div 0
        public Dictionary<string, Dictionary<ModifierOperationType, float>> CalculateModifiers() 
        {
            var totalModifies = new Dictionary<string, Dictionary<ModifierOperationType, float>>();

            foreach (var modifier in Configs.Modifiers) {
                // 当前attributeType的条目是否存在
                if (!totalModifies.TryGetValue(modifier.Type, out var typeModifiers)) {
                    typeModifiers = new Dictionary<ModifierOperationType, float>();
                    totalModifies.Add(modifier.Type, typeModifiers);
                }
                // 当前attribute的operation条目是否存在
                if (!typeModifiers.TryGetValue(modifier.OperationType, out var value)) {
                    value = 0;
                    switch (modifier.OperationType) {
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
                    typeModifiers.Add(modifier.OperationType, value);
                }

                switch (modifier.OperationType) {
                    case ModifierOperationType.Add:
                        totalModifies[modifier.Type][modifier.OperationType] += modifier.ScaledMagnitude;
                        break;
                    case ModifierOperationType.Multiply:
                        totalModifies[modifier.Type][modifier.OperationType] *= modifier.ScaledMagnitude;
                        break;
                    case ModifierOperationType.Divide:
                        totalModifies[modifier.Type][modifier.OperationType] *= modifier.ScaledMagnitude;
                        break;
                }
            }
            return totalModifies;
        }

        // e.g. HP -> oldValue 100 newValue 200
        //      MP -> oldValue 200 newValue 189
        public Dictionary<string, ModifyArrtibuteValues> CalculateAttributes(
                            AbilitySystem target, 
                            Dictionary<string, Dictionary<ModifierOperationType, float>> totalModifies, 
                            bool operateOnCurrentValue = false
        ) {
            var totalAttributeChange = new Dictionary<string, ModifyArrtibuteValues>();

            foreach (var modifyOfType in totalModifies) {
                if (!modifyOfType.Value.TryGetValue(ModifierOperationType.Add, out var addition)) {
                    addition = 0;
                }
                if (!modifyOfType.Value.TryGetValue(ModifierOperationType.Multiply, out var multiplication)) {
                    multiplication = 1;
                }
                if (!modifyOfType.Value.TryGetValue(ModifierOperationType.Divide, out var division)) {
                    division = 1;
                }

                float oldValue = 0f;
                if (!operateOnCurrentValue) {
                    oldValue = target.GetBaseValue(modifyOfType.Key);
                } else {
                    oldValue = target.GetCurrentValue(modifyOfType.Key);
                }
                float newValue = (oldValue + addition) * (multiplication / division);

                if (!totalAttributeChange.TryGetValue(modifyOfType.Key, out ModifyArrtibuteValues values)) {
                    values = new ModifyArrtibuteValues();
                    totalAttributeChange.Add(modifyOfType.Key, values);
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
                var aggregators = target.ActivedEffects.GetAggregatorsForAttribute(changeByType.Key);
                if (aggregators.Count() != 0) {
                    target.ActivedEffects.UpdateAttribute(aggregators, changeByType.Key);
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

