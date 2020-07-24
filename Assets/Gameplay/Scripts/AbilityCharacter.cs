using System.Collections.Generic;
using GameplayAbilitySystem;
using UnityEngine;
using System;

namespace AbilitySystemDemo 
{
    public class AbilityCharacter : MonoBehaviour
    {
        public Dictionary<string, Ability> Abilities { get; private set; } = new Dictionary<string, Ability>();
        public AbilitySystem AbilitySystem { get; private set; }

        private void Awake()
        {
            AbilitySystem = GetComponent<AbilitySystem>();
            // 开始游戏之前根据选好的技能初始化技能相关数据.
            Abilities.Add(ID.ability_fire,      Ability.Get(ID.ability_fire));
            Abilities.Add(ID.ability_bloodPact, Ability.Get(ID.ability_bloodPact));
            Abilities.Add(ID.ability_heal,      Ability.Get(ID.ability_heal));
            // Abilities.Add("fire", TestData.GetAbility("fire"));
            // Abilities.Add("bloodPact", TestData.GetAbility("bloodPact"));
            // Abilities.Add("heal", TestData.GetAbility("heal"));
        }

        public void CastAbility(string id)
        {
            AbilitySystem target = null;
            // 释放技能之前先确定好目标, 要补充没有目标的情况下技能的表现
            if (id == ID.ability_fire) {
                target = GameObject.FindGameObjectWithTag("Enemy").GetComponent<AbilitySystem>();
            } else {
                target = AbilitySystem;
            }
            AbilitySystem.TryActivateAbility(Abilities[id], target);
        }

        private void Update() {
            if (Input.GetButtonUp("Ability 1")) {
                CastAbility(ID.ability_fire);
            } 
            else if (Input.GetButtonUp("Ability 2")) {
                CastAbility(ID.ability_bloodPact);
            }   
            else if (Input.GetButtonUp("Ability 3")) {
                CastAbility(ID.ability_heal);
            }
        }
    }
}