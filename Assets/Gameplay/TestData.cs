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
        private static Dictionary<string, EffectConfigs> coolDownConfig = new Dictionary<string, EffectConfigs>();
        private static Dictionary<string, EffectConfigs> costConfig = new Dictionary<string, EffectConfigs>();
        private static Dictionary<string, EffectConfigs> normalConfig = new Dictionary<string, EffectConfigs>();
        private static Dictionary<string, Effect> effect = new Dictionary<string, Effect>();

        private static Dictionary<string, InstantAttack> instantAttack = new Dictionary<string, InstantAttack>();
        private static Dictionary<string, AbilityLogic> logic = new Dictionary<string, AbilityLogic>();
        private static Dictionary<string, Ability> ability = new Dictionary<string, Ability>();
        static TestData() 
        {
            cueActions.Add("manaSurgeZ",            GetSpawnCueAction("EnergyExplosionRay", 2));
            cueActions.Add("regenHealthSprite",     GetSpawnCueAction("Sprites", 3));
            cueActions.Add("regenHealth",           GetSpawnCueAction("MagicCircle", 1));
            cueActions.Add("spawnBigExplosion",     GetSpawnCueAction("FireExplosion", 2));
            cueActions.Add("spawnEnergyExplosion",  GetSpawnCueAction("EnergyExplosion", 2));
            cueActions.Add("spawnMagicCircle",      GetSpawnCueAction("MagicCircle", 3));

            // cues.Add("fire",        GetEffectCues(cueActions["spawnBigExplosion"], 
            //                                         cueActions["spawnBigExplosion"], 
            //                                         cueActions["spawnBigExplosion"]));

            // cues.Add("heal",        GetEffectCues(cueActions["spawnMagicCircle"]));

            // cues.Add("mana",        GetEffectCues(cueActions["spawnEnergyExplosion"], 
            //                                         null, 
            //                                        cueActions["spawnEnergyExplosion"]));

            // cues.Add("regenHealth", GetEffectCues(cueActions["regenHealthSprite"], 
            //                                         cueActions["regenHealthSprite"], 
            //                                         null));

            // cues.Add("regenMana",   GetEffectCues(cueActions["manaSurgeZ"], 
            //                                         cueActions["manaSurgeZ"], 
            //                                        null));

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

            normalConfig.Add("regenMana", GetNormalEffectConfigs(
                modifiers: new List<ModifierConfig>() {
                    new ModifierConfig(AttributeType.Mana, OperationType.Add, 10)
                },
                cues: new EffectCues(
                    cueActions["manaSurgeZ"], cueActions["manaSurgeZ"], null
                )
            ));
            effect.Add("regenMana", GetEffect(normalConfig["regenMana"]));

            normalConfig.Add("regenHealth", GetNormalEffectConfigs(
                modifiers: new List<ModifierConfig>() {
                    new ModifierConfig(AttributeType.Health, OperationType.Add, 10)
                },
                cues: new EffectCues(
                    cueActions["regenHealthSprite"], cueActions["regenHealthSprite"], null
                )
            ));
            effect.Add("regenHealth", GetEffect(normalConfig["regenHealth"]));

            normalConfig.Add("heal", GetNormalEffectConfigs(
                iconKey: "HealIcon",
                durationPolicy: DurationPolicy.Duration, duration: 20,
                period: 3, isExecuteOnApply: false, effectOnExecute: effect["regenHealth"],
                removeEffectsInfo: new List<RemoveEffectInfo>() {
                    new RemoveEffectInfo(100, 1)
                },
                cues: new EffectCues(
                    cueActions["spawnMagicCircle"], null, null
                )
            ));
            effect.Add("heal", GetEffect(normalConfig["heal"]));

            normalConfig.Add("fire", GetNormalEffectConfigs(
                iconKey: "FireIcon",
                modifiers: new List<ModifierConfig>() {
                    new ModifierConfig(AttributeType.Health, OperationType.Add, -5)
                },
                cues: new EffectCues(
                    cueActions["spawnBigExplosion"], cueActions["spawnBigExplosion"], cueActions["spawnBigExplosion"]
                )
            ));
            effect.Add("fire", GetEffect(normalConfig["heal"]));

            normalConfig.Add("bloodPact", GetNormalEffectConfigs(
                iconKey: "ManaIcon",
                durationPolicy: DurationPolicy.Duration, duration: 20,
                period: 2, isExecuteOnApply: true, effectOnExecute: effect["regenMana"],
                stackType: StackType.StackBySource, maxStack: 2, stackExpirationPolicy: StackExpirationPolicy.RemoveSingleStackAndRefreshDuration,
                cues: new EffectCues(
                    cueActions["spawnEnergyExplosion"], null, cueActions["spawnEnergyExplosion"]
                )
            ));
            effect.Add("bloodPact", GetEffect(normalConfig["bloodPact"]));

            logic.Add("fire", GetTrackingLogic("Fireball", new Vector3(0, 1.5f, 0), effect["fire"]));
            logic.Add("bloodPact", GetInstantLogic( true, effect["bloodPact"]));
            logic.Add("heal", GetInstantLogic(false, effect["heal"]));

            ability.Add("fire", CreateAbility(
                "ManaIcon", 
                effect["cost_mana"], 
                new List<Effect>() {effect["cd_bloodPact"], effect["cd_global"] },
                logic["bloodPact"]
            ));

            ability.Add("heal", CreateAbility(
                "HealIcon",
                effect["cost_mana"],
                new List<Effect>() { effect["cd_heal"], effect["cd_global"]},
                logic["heal"]
            ));

            ability.Add("bloodPact", CreateAbility(
                "ManaIcon",
                effect["cost_health"],
                new List<Effect>() { effect["cd_bloodPact"], effect["cd_global"]},
                logic["bloodPact"]
            ));
        }


        public static Ability GetAbility(string id)
        {
            return ability[id];
        }

        private static Ability CreateAbility(
            string iconKey,
            Effect cost,
            List<Effect> coolDown,
            AbilityLogic logic
        )
        {
            Ability ability = ScriptableObject.CreateInstance("Ability") as Ability;
            ability.Id = GetId();
            ability.IconKey = iconKey;
            ability.CostEffect = cost;
            ability.CooldownEffects = coolDown;
            ability.AbilityLogic = logic;
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

        private static AbilityLogic GetInstantLogic(bool wait, Effect effect)
        {
            InstantAttack instant = ScriptableObject.CreateInstance("InstantAttack") as InstantAttack;
            instant.SetData(wait, effect);
            return instant;
        }

        private static AbilityLogic GetTrackingLogic(string projectileKey, Vector3 offset, Effect effect)
        {
            TrackingAttack tracking = ScriptableObject.CreateInstance("TrackingAttack") as TrackingAttack;
            tracking.SetData(projectileKey, offset, effect);
            return tracking;
        }



        private static EffectConfigs GetNormalEffectConfigs(
            EffectCues cues,
            string iconKey = "",
            DurationPolicy durationPolicy = DurationPolicy.Instant,
            float duration = 0,
            float period = 0,
            bool isExecuteOnApply = false,
            Effect effectOnExecute = null,
            StackType stackType = StackType.None,
            int maxStack = 0,
            StackExpirationPolicy stackExpirationPolicy = StackExpirationPolicy.ClearEntireStack,
            List<ModifierConfig> modifiers = null,
            List<RemoveEffectInfo> removeEffectsInfo = null
        ) {
            EffectConfigs config = new EffectConfigs();
            config.Id = GetId();
            config.IconKey = iconKey;
            config.EffectType = EffectType.Normal;
            config.DurationConfig = new DurationConfig(durationPolicy, duration);
            config.PeriodConfig = new PeriodConfig(period, isExecuteOnApply, effectOnExecute);
            config.StackConfig = new StackConfig(stackType, maxStack, stackExpirationPolicy);
            config.Modifiers = modifiers;
            config.RemoveEffectsInfo = removeEffectsInfo;
            config.EffectCues = cues;
            return config;
        }

        private static EffectConfigs GetCoolDownConfig(bool isGlobal, float duration)
        {
            EffectConfigs config = new EffectConfigs();
            config.Id = GetId();
            config.EffectType = isGlobal ? EffectType.GlobalCoolDown : EffectType.CoolDown;
            config.DurationConfig = new DurationConfig(DurationPolicy.Duration, duration);
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

    }
}