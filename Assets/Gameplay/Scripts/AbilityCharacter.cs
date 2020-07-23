using System.Collections.Generic;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Abilities;
using UnityEngine;
using System;
using UnityEngine.Serialization;
using System.Threading.Tasks;
namespace AbilitySystemDemo 
{
    public class AbilityCharacter : MonoBehaviour
    {
        [Serializable]
        public class CastingAbilityContainer
        {
            public Ability ability;
            public AbilitySystem target;
        }

        public List<CastingAbilityContainer> abilities = new List<CastingAbilityContainer>();
        public AbilitySystem AbilitySystem { get; private set; }

        private void  Awake()
        {
            AbilitySystem = GetComponent<AbilitySystem>();
        }

        private void Start() {
            abilities = new List<CastingAbilityContainer>() {
                new CastingAbilityContainer() {
                    ability = TestData.GetAbility("fire"),
                    target = GameObject.FindGameObjectWithTag("Enemy").GetComponent<AbilitySystem>(),
                },
                new CastingAbilityContainer() {
                    ability = TestData.GetAbility("bloodPact"),
                    target = AbilitySystem,
                },
                new CastingAbilityContainer() {
                    ability = TestData.GetAbility("heal"),
                    target = AbilitySystem,
                },
            };
        }

        public void CastAbility(int n)
        {
            if (n >= abilities.Count) return;
            if (abilities[n] == null) return;
            if (abilities[n].ability == null) return;
            if (abilities[n].target == null) return;

            Ability ability = abilities[n].ability;
            AbilitySystem target = abilities[n].target;
            AbilitySystem.TryActivateAbility(ability, target);
        }
    }
}