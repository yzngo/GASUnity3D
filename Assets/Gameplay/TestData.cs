using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Abilities;
using GameplayAbilitySystem.Effects;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

namespace AbilitySystemDemo
{
    public static class TestData
    {
        private static int id = 1000;
        public static int GetId() => id++;

        private static Dictionary<string, SpawnCueAction> cueActions = new Dictionary<string, SpawnCueAction>();
        private static Dictionary<string, EffectCues> cues = new Dictionary<string, EffectCues>();
        private static Dictionary<string, EffectConfigs> coolDownConfig = new Dictionary<string, EffectConfigs>();
        private static Dictionary<string, EffectConfigs> costConfig = new Dictionary<string, EffectConfigs>();
        private static Dictionary<string, EffectConfigs> normalConfig = new Dictionary<string, EffectConfigs>();
        private static Dictionary<string, Effect> effect = new Dictionary<string, Effect>();
        static TestData() 
        {
            cueActions.Add("manaSurgeZ",            GetSpawnCueAction("EnergyExplosionRay", 2));
            cueActions.Add("regenHealthSprite",     GetSpawnCueAction("Sprites", 3));
            cueActions.Add("regenHealth",           GetSpawnCueAction("MagicCircle", 1));
            cueActions.Add("spawnBigExplosion",     GetSpawnCueAction("FireExplosion", 2));
            cueActions.Add("spawnEnergyExplosion",  GetSpawnCueAction("EnergyExplosion", 2));
            cueActions.Add("spawnMagicCircle",      GetSpawnCueAction("MagicCircle", 3));

            cues.Add("fire",        GetEffectCues(cueActions["spawnBigExplosion"], cueActions["spawnBigExplosion"], cueActions["spawnBigExplosion"]));
            cues.Add("heal",        GetEffectCues(cueActions["spawnMagicCircle"]));
            cues.Add("mana",        GetEffectCues(cueActions["spawnEnergyExplosion"], null, cueActions["spawnEnergyExplosion"]));
            cues.Add("regenHealth", GetEffectCues(cueActions["regenHealthSprite"], cueActions["regenHealthSprite"], null));
            cues.Add("regenMana",   GetEffectCues(cueActions["manaSurgeZ"], cueActions["manaSurgeZ"], null));

            coolDownConfig.Add("bloodPact", GetCoolDownConfig(isGlobal:false, duration:2));
            coolDownConfig.Add("fire",      GetCoolDownConfig(isGlobal:false, duration:3));
            coolDownConfig.Add("global",    GetCoolDownConfig(isGlobal:true,  duration:1));
            coolDownConfig.Add("heal",      GetCoolDownConfig(isGlobal:false, duration:2));

            costConfig.Add("health", GetCostConfig(AttributeType.Health, OperationType.Add, -10));
            costConfig.Add("mana", GetCostConfig(AttributeType.Mana, OperationType.Add, -5));

// -----------------------------------------------------------------------------------

            effect.Add("cd_bloodPact",     GetEffect(coolDownConfig["bloodPact"]));
            effect.Add("cd_fire",          GetEffect(coolDownConfig["fire"]));
            effect.Add("cd_global",        GetEffect(coolDownConfig["global"]));
            effect.Add("cd_heal",          GetEffect(coolDownConfig["heal"]));

            effect.Add("cost_health",       GetEffect(costConfig["health"]));
            effect.Add("cost_mana",         GetEffect(costConfig["mana"]));

            // normalConfig.Add("regenMana", GetNormalEffectConfigs(
            //     string.Empty(),

            // ));
            // normalConfig.Add("bloodPact",   GetNormalEffectConfigs(
                // "ManaIcon", 
                // DurationPolicy.Duration, 20.0f,
                // 2, true, 
            // ));

        }
        public static async Task<Ability> CreateAbility(string abilityId)
        {
            var ability = ScriptableObject.CreateInstance("Ability") as Ability;
            if(abilityId == "fire") {
                ability.Id = 1;
                ability.Icon = await Addressables.LoadAssetAsync<Sprite>("FireIcon").Task;

            } else if (abilityId == "bloodPact") {

            } else if (abilityId == "heal") {
                
            }
            return ability;
        }

        private static SpawnCueAction GetSpawnCueAction(string prefab, float destroyTime)
        {
            SpawnCueAction action = ScriptableObject.CreateInstance("SpawnCueAction") as SpawnCueAction;
            action.objectKey = prefab;
            action.ResetDestroyTime(destroyTime);
            return action;
        }

        private static Effect GetEffect(EffectConfigs configs)
        {
            Effect effect = ScriptableObject.CreateInstance("Effect") as Effect;
            effect.Configs = configs;
            return effect;
        }

        private static async Task<EffectConfigs> GetNormalEffectConfigs(
            string iconKey,
            DurationPolicy durationPolicy,
            float duration,
            float period,
            bool isExecuteOnApply,
            Effect effectOnExecute,
            StackType stackType,
            int maxStack,
            StackExpirationPolicy stackExpirationPolicy,
            List<ModifierConfig> modifiers,
            List<RemoveEffectInfo> removeEffectsInfo,
            List<EffectCues> cues
        ) {
            EffectConfigs config = new EffectConfigs();
            config.Id = GetId();
            config.Icon = await Addressables.LoadAssetAsync<Sprite>(iconKey).Task;
            config.EffectType = EffectType.Normal;
            config.DurationConfig = new DurationConfig(durationPolicy, duration);
            config.PeriodConfig = new PeriodConfig(period, isExecuteOnApply, effectOnExecute);
            config.StackConfig = new StackConfig(stackType, maxStack, stackExpirationPolicy);
            config.Modifiers = modifiers;
            config.RemoveEffectsInfo = removeEffectsInfo;
            config.Cues = cues;
            return config;
        }

        private static EffectConfigs GetCoolDownConfig(bool isGlobal, float duration)
        {
            EffectConfigs config = new EffectConfigs();
            config.Id = GetId();
            config.EffectType = isGlobal ? EffectType.GlobalCoolDown : EffectType.CoolDown;
            config.DurationConfig.Policy = DurationPolicy.Duration;
            config.DurationConfig.Duration = duration;
            return config;
        }

        private static EffectConfigs GetCostConfig(string attributeType, OperationType operation, float cost)
        {
            EffectConfigs config = new EffectConfigs();
            config.Id = GetId();
            config.EffectType = EffectType.Cost;
            config.Modifiers = new List<ModifierConfig>() { 
                new ModifierConfig(attributeType, operation, cost)
            };
            return config;
        }

        private static EffectCues GetEffectCues(BaseCueAction onActive = null, BaseCueAction onExecute = null, BaseCueAction onRemove = null)
        {
            EffectCues cues = ScriptableObject.CreateInstance("EffectCues") as EffectCues;
            cues.Reset(onActive, onExecute, onRemove);
            return cues;
        }

    }
}