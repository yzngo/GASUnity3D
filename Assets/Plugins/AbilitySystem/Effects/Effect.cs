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
        [SerializeField] private EffectConfigs configs = default;
        public EffectConfigs Configs => configs;

        public Dictionary<string, Dictionary<OperationType, float>> GetAllOperation() 
        {
            var allOperation = new Dictionary<string, Dictionary<OperationType, float>>();

            foreach (ModifierConfig modifier in Configs.Modifiers) {

                if (!allOperation.TryGetValue(modifier.Type, out Dictionary<OperationType, float> operation)) {
                    operation = new Dictionary<OperationType, float>();
                    allOperation.Add(modifier.Type, operation);
                }

                if (!operation.TryGetValue(modifier.OperationType, out float value)) {
                    value = 0;
                    switch (modifier.OperationType) {
                        case OperationType.Multiply:
                            value = 1;
                            break;
                        case OperationType.Divide:
                            value = 1;
                            break;
                        default:
                            value = 0;
                            break;
                    }
                    operation.Add(modifier.OperationType, value);
                }

                switch (modifier.OperationType) {
                    case OperationType.Add:
                        allOperation[modifier.Type][modifier.OperationType] += modifier.Value;
                        break;
                    case OperationType.Multiply:
                        allOperation[modifier.Type][modifier.OperationType] *= modifier.Value;
                        break;
                    case OperationType.Divide:
                        allOperation[modifier.Type][modifier.OperationType] *= modifier.Value;
                        break;
                }
            }
            return allOperation;
        }

        public List<AttributeModifyInfo> GetAllModify(
                            AbilitySystem target, 
                            Dictionary<string, Dictionary<OperationType, float>> allOperation, 
                            bool operateOnCurrentValue = false) 
        {
            var allModify = new List<AttributeModifyInfo>();

            foreach (var operation in allOperation) {
                if (!operation.Value.TryGetValue(OperationType.Add, out float addition)) {
                    addition = 0;
                }
                if (!operation.Value.TryGetValue(OperationType.Multiply, out float multiplication)) {
                    multiplication = 1;
                }
                if (!operation.Value.TryGetValue(OperationType.Divide, out float division)) {
                    division = 1;
                }

                float oldValue = 0f;
                if (!operateOnCurrentValue) {
                    oldValue = target.GetBaseValue(operation.Key);
                } else {
                    oldValue = target.GetCurrentValue(operation.Key);
                }
                float newValue = (oldValue + addition) * (multiplication / division);

                AttributeModifyInfo values = allModify.Find(x => x.Type == operation.Key);
                if (values == null) {
                    values = new AttributeModifyInfo();
                    allModify.Add(values);
                }
                values.Type = operation.Key;
                values.NewValue += newValue;
                values.OldValue += oldValue;
            }
            return allModify;
        }

        public void InstantApplyTo(AbilitySystem target) 
        {
            Dictionary<string, Dictionary<OperationType, float>> allOperation = this.GetAllOperation();
            List<AttributeModifyInfo> allModify = this.GetAllModify(target, allOperation);

            // For each attribute, apply the new modified values
            foreach (AttributeModifyInfo modify in allModify) {
                target.SetBaseValue(modify.Type, modify.NewValue);

                IEnumerable<AttributeOperationContainer> operation = target.ActivedEffects.GetAllOperationFor(modify.Type);
                if (operation.Count() != 0) {
                    target.ActivedEffects.UpdateAttribute(operation, modify.Type);
                } else {
                    target.SetCurrentValue(modify.Type, target.GetBaseValue(modify.Type));
                }
            }
        }
    }

    public class AttributeModifyInfo {
        public string Type { get; set; }
        public float OldValue { get; set; }
        public float NewValue { get; set; }
    }
}