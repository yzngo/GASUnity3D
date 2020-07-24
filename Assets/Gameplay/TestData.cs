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
        // private static Dictionary<string, SpawnCueAction> cueActions = new Dictionary<string, SpawnCueAction>();
        private static Dictionary<string, EffectConfigs> coolDownConfig = new Dictionary<string, EffectConfigs>();
        private static Dictionary<string, EffectConfigs> costConfig = new Dictionary<string, EffectConfigs>();
        private static Dictionary<string, EffectConfigs> normalConfig = new Dictionary<string, EffectConfigs>();
        private static Dictionary<string, Effect> effect = new Dictionary<string, Effect>();

        private static Dictionary<string, InstantAttack> instantAttack = new Dictionary<string, InstantAttack>();
        private static Dictionary<string, AbilityLogic> logic = new Dictionary<string, AbilityLogic>();
        private static Dictionary<string, Ability> abilities = new Dictionary<string, Ability>();
        static TestData() 
        {

// init cue action -----------------------------------------------------------------------
            // cueActions.Add("manaSurgeZ",            CreateSpawnCueAction("EnergyExplosionRay", 2));
            // cueActions.Add("regenHealthSprite",     CreateSpawnCueAction("Sprites", 3));
            // cueActions.Add("regenHealth",           CreateSpawnCueAction("MagicCircle", 1));
            // cueActions.Add("spawnBigExplosion",     CreateSpawnCueAction("FireExplosion", 2));
            // cueActions.Add("spawnEnergyExplosion",  CreateSpawnCueAction("EnergyExplosion", 2));
            // cueActions.Add("spawnMagicCircle",      CreateSpawnCueAction("MagicCircle", 3));

// init effect config --------------------------------------------------------------------
            coolDownConfig.Add("bloodPact", CreateCoolDownConfig(id:1, isGlobal:false, duration:2));
            coolDownConfig.Add("fire",      CreateCoolDownConfig(id:2, isGlobal:false, duration:3));
            coolDownConfig.Add("global",    CreateCoolDownConfig(id:3, isGlobal:true,  duration:1));
            coolDownConfig.Add("heal",      CreateCoolDownConfig(id:4, isGlobal:false, duration:2));

            costConfig.Add("health",        CreateCostConfig(10, AttributeType.Health, OperationType.Add, -10));
            costConfig.Add("mana",          CreateCostConfig(11, AttributeType.Mana, OperationType.Add, -5));

// init effect  --------------------------------------------------------------------------

            effect.Add("cd_bloodPact",     CreateEffect(coolDownConfig["bloodPact"]));
            effect.Add("cd_fire",          CreateEffect(coolDownConfig["fire"]));
            effect.Add("cd_global",        CreateEffect(coolDownConfig["global"]));
            effect.Add("cd_heal",          CreateEffect(coolDownConfig["heal"]));

            effect.Add("cost_health",      CreateEffect(costConfig["health"]));
            effect.Add("cost_mana",        CreateEffect(costConfig["mana"]));

            normalConfig.Add("regenMana", CreateNormalEffectConfigs(
                100,
                modifiers: new List<ModifierConfig>() {
                    new ModifierConfig(AttributeType.Mana, OperationType.Add, 10)
                },
                cues: new EffectCues(
                    SpawnCueAction.Get(ID.manaSurgeZ),
                    SpawnCueAction.Get(ID.manaSurgeZ),
                    null
                )
            ));
            effect.Add("regenMana", CreateEffect(normalConfig["regenMana"]));

            normalConfig.Add("regenHealth", CreateNormalEffectConfigs(
                101,
                modifiers: new List<ModifierConfig>() {
                    new ModifierConfig(AttributeType.Health, OperationType.Add, 10)
                },
                cues: new EffectCues(
                    SpawnCueAction.Get(ID.regenHealthSprite),
                    SpawnCueAction.Get(ID.regenHealthSprite),
                    null
                )
            ));
            effect.Add("regenHealth", CreateEffect(normalConfig["regenHealth"]));

            normalConfig.Add("heal", CreateNormalEffectConfigs(
                102,
                iconKey: "HealIcon",
                durationPolicy: DurationPolicy.Duration, duration: 20,
                period: 3, isExecuteOnApply: false, effectOnExecute: effect["regenHealth"],
                removeEffectsInfo: new List<RemoveEffectInfo>() {
                    new RemoveEffectInfo(100, 1)
                },
                cues: new EffectCues(
                    SpawnCueAction.Get(ID.spawnMagicCircle),null,null
                )
            ));
            effect.Add("heal", CreateEffect(normalConfig["heal"]));

            normalConfig.Add("fire", CreateNormalEffectConfigs(
                103,
                iconKey: "FireIcon",
                modifiers: new List<ModifierConfig>() {
                    new ModifierConfig(AttributeType.Health, OperationType.Add, -5)
                },
                cues: new EffectCues(
                    SpawnCueAction.Get(ID.spawnBigExplosion),
                    SpawnCueAction.Get(ID.spawnBigExplosion),
                    SpawnCueAction.Get(ID.spawnBigExplosion)
                )
            ));
            effect.Add("fire", CreateEffect(normalConfig["fire"]));

            normalConfig.Add("bloodPact", CreateNormalEffectConfigs(
                104,
                iconKey: "ManaIcon",
                durationPolicy: DurationPolicy.Duration, duration: 20,
                period: 2, isExecuteOnApply: true, effectOnExecute: effect["regenMana"],
                stackType: StackType.StackBySource, maxStack: 2, stackExpirationPolicy: StackExpirationPolicy.RemoveSingleStackAndRefreshDuration,
                cues: new EffectCues(
                    SpawnCueAction.Get(ID.spawnEnergyExplosion),
                    null,
                    SpawnCueAction.Get(ID.spawnEnergyExplosion)
                )
            ));
            effect.Add("bloodPact", CreateEffect(normalConfig["bloodPact"]));

            logic.Add("fire", GetTrackingLogic("Fireball", new Vector3(0, 1.5f, 0), effect["fire"]));
            logic.Add("bloodPact", GetInstantLogic( true, effect["bloodPact"]));
            logic.Add("heal", GetInstantLogic(false, effect["heal"]));

            abilities.Add("fire", CreateAbility(
                1000,
                "FireIcon", 
                effect["cost_mana"], 
                new List<Effect>() {effect["cd_fire"], effect["cd_global"] },
                logic["fire"]
            ));

            abilities.Add("heal", CreateAbility(
                1001,
                "HealIcon",
                effect["cost_mana"],
                new List<Effect>() { effect["cd_heal"], effect["cd_global"]},
                logic["heal"]
            ));

            abilities.Add("bloodPact", CreateAbility(
                1002,
                "ManaIcon",
                effect["cost_health"],
                new List<Effect>() { effect["cd_bloodPact"], effect["cd_global"]},
                logic["bloodPact"]
            ));
        }

        public static Ability GetAbility(string id)
        {
            // if (!abilities.TryGetValue(id, out Ability ability)) {
            //     CreateAbility(id);
            // }
            return abilities[id];
        }

        private static Ability CreateAbility(
            int id,
            string iconKey,
            Effect cost,
            List<Effect> coolDown,
            AbilityLogic logic
        )
        {
            Ability ability = ScriptableObject.CreateInstance("Ability") as Ability;
            ability.Id = id;
            ability.IconKey = iconKey;
            ability.CostEffect = cost;
            ability.CooldownEffects = coolDown;
            ability.AbilityLogic = logic;
            return ability;
        }

        // private static SpawnCueAction CreateSpawnCueAction(string prefab, float destroyTime)
        // {
        //     SpawnCueAction action = ScriptableObject.CreateInstance("SpawnCueAction") as SpawnCueAction;
        //     action.objectKey = prefab;
        //     action.ResetDestroyTime(destroyTime);
        //     return action;
        // }

        private static Effect CreateEffect(EffectConfigs configs)
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



        private static EffectConfigs CreateNormalEffectConfigs(
            int id,
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
            config.Id = id;
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

        private static EffectConfigs CreateCoolDownConfig(int id, bool isGlobal, float duration)
        {
            EffectConfigs config = new EffectConfigs();
            config.Id = id;
            config.EffectType = isGlobal ? EffectType.GlobalCoolDown : EffectType.CoolDown;
            config.DurationConfig = new DurationConfig(DurationPolicy.Duration, duration);
            return config;
        }

        private static EffectConfigs CreateCostConfig(int id,string attributeType, OperationType operation, float cost)
        {
            EffectConfigs config = new EffectConfigs();
            config.Id = id;
            config.EffectType = EffectType.Cost;
            config.Modifiers = new List<ModifierConfig>() { 
                new ModifierConfig(attributeType, operation, cost)
            };
            return config;
        }

    }
}