using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameplayAbilitySystem
{
    [CreateAssetMenu(fileName = "Gameplay Effect", menuName = "Ability System/Effect")]
    public class Effect : ScriptableObject 
    {
        [SerializeField] private EffectConfigs configs = default;
        public EffectConfigs Configs { get => configs; set { configs = value;}}

        public Dictionary<string, Dictionary<OperationType, float>> GetAllOperation() 
        {
            var allOperation = new Dictionary<string, Dictionary<OperationType, float>>();

            foreach (ModifierConfig modifier in Configs.Modifiers) {

                if (!allOperation.TryGetValue(modifier.AttributeType, out Dictionary<OperationType, float> operation)) {
                    operation = new Dictionary<OperationType, float>();
                    allOperation.Add(modifier.AttributeType, operation);
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
                        allOperation[modifier.AttributeType][modifier.OperationType] += modifier.Value;
                        break;
                    case OperationType.Multiply:
                        allOperation[modifier.AttributeType][modifier.OperationType] *= modifier.Value;
                        break;
                    case OperationType.Divide:
                        allOperation[modifier.AttributeType][modifier.OperationType] *= modifier.Value;
                        break;
                }
            }
            return allOperation;
        }

        public List<AttributeModifyInfo> GetAllModifyInfo(
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

                AttributeModifyInfo values = allModify.Find(x => x.AttributeType == operation.Key);
                if (values == null) {
                    values = new AttributeModifyInfo();
                    allModify.Add(values);
                }
                values.AttributeType = operation.Key;
                values.NewValue += newValue;
                values.OldValue += oldValue;
            }
            return allModify;
        }

        public void InstantApplyTo(AbilitySystem target) 
        {
            Dictionary<string, Dictionary<OperationType, float>> allOperation = this.GetAllOperation();
            List<AttributeModifyInfo> allModify = this.GetAllModifyInfo(target, allOperation);

            foreach (AttributeModifyInfo modify in allModify) {
                target.SetBaseValue(modify.AttributeType, modify.NewValue);
                target.ReEvaluateCurrentValueFor(modify.AttributeType);
            }
        }

        private static Dictionary<string, Effect> effect = new Dictionary<string, Effect>();

        public static Effect Get(EffectConfigs config)
        {
            Effect effect = ScriptableObject.CreateInstance("Effect") as Effect;
            effect.configs = config;
            return effect;
        }
    }

    public class AttributeModifyInfo {
        public string AttributeType { get; set; }
        public float OldValue { get; set; }
        public float NewValue { get; set; }
    }
}