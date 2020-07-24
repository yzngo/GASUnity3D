using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

namespace GameplayAbilitySystem
{
    public static class AddressKey {
        public static string EnergyExplosionRay = "EnergyExplosionRay";
        public static string Sprites = "Sprites";
        public static string MagicCircle= "MagicCircle";
        public static string FireExplosion= "FireExplosion";
        public static string EnergyExplosion= "EnergyExplosion";

        public static string HealIcon = "HealIcon";
        public static string FireIcon = "FireIcon";
        public static string ManaIcon = "ManaIcon";

        public static string Fireball = "Fireball";
    }
    
    // id 之后全部改成读表
    public static partial class ID 
    {
    // cue
        public const string cue_manaSurgeZ= "manaSurgeZ";
        public const string cue_regenHealthSprite= "regenHealthSprite";
        public const string cue_regenHealth= "regenHealth";
        public const string cue_spawnBigExplosion= "spawnBigExplosion";
        public const string cue_spawnEnergyExplosion= "spawnEnergyExplosion";
        public const string cue_spawnMagicCircle= "spawnMagicCircle";

    // ability
        public const string ability_fire = "ability_fire";
        public const string ability_bloodPact = "ability_bloodPact";
        public const string ability_heal = "ability_heal";

    // effect 
        public const string effect_regenMana = "effect_regenMana";
        public const string effect_regenHealth = "effect_regenHealth";
        public const string effect_heal = "effect_heal";
        public const string effect_fire = "effect_fire";
        public const string effect_bloodPact = "effect_bloodPact";
    }

    public static class KV {
        public static Dictionary<string, string> effectId_SpriteKey = new Dictionary<string, string>();
        public static Dictionary<string, string> abilityId_SpriteKey = new Dictionary<string, string>();

        static KV() {
            effectId_SpriteKey.Add(ID.effect_fire, AddressKey.FireIcon);
            effectId_SpriteKey.Add(ID.effect_heal, AddressKey.HealIcon);
            effectId_SpriteKey.Add(ID.effect_bloodPact, AddressKey.ManaIcon);

            abilityId_SpriteKey.Add(ID.ability_fire, AddressKey.FireIcon);
            abilityId_SpriteKey.Add(ID.ability_heal, AddressKey.HealIcon);
            abilityId_SpriteKey.Add(ID.ability_bloodPact, AddressKey.ManaIcon);
        }
    }
}