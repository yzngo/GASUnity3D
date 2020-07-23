using System.Linq;
using System.Collections.Generic;
using GameplayAbilitySystem.Abilities;
using GameplayAbilitySystem.Effects;
using UnityEngine;
using GameplayAbilitySystem.Attributes;
using GameplayAbilitySystem.Utility;

namespace GameplayAbilitySystem 
{
    [AddComponentMenu("Ability System/Ability System")]
    public class AbilitySystem : MonoBehaviour 
    {
        [SerializeField] private Transform targetPoint = default;
        public Transform TargetPoint => targetPoint;

        private AbilityEvent onAbilityStart = new AbilityEvent();
        public AbilityEvent OnAbilityStart => onAbilityStart;

        private AnimEvent onAnimEvent = new AnimEvent();
        public AnimEvent OnAnimEvent => onAnimEvent;

        private List<Ability> runningAbilities = new List<Ability>();

        private ActivedEffects activedEffects;
        public ActivedEffects ActivedEffects => activedEffects;

        private Animator animator;
        public Animator Animator => animator;
        private AttributeSet attributeSet = new AttributeSet();
        
        public void Awake() {
            activedEffects = new ActivedEffects(this);
            animator = GetComponent<Animator>();

            AddAttribute(AttributeType.MaxHealth, 100, 100);
            AddAttribute(AttributeType.Health, 100, 100);
            AddAttribute(AttributeType.MaxMana, 50, 50);
            AddAttribute(AttributeType.Mana, 50, 50);
            AddAttribute(AttributeType.Speed, 5, 5);
        }

        public bool TryActivateAbility(Ability ability, AbilitySystem target = null) {

            if (runningAbilities.Contains(ability)) {
                return false;
            }
            if (!ability.IsActivatable(this)) {
                return false;
            }

            runningAbilities.Add(ability);
            ability.Commit(this);

            var data = new AbilityEventData();
            data.abilityId = ability.Id;
            data.ability = ability;
            data.target = target;
            OnAbilityStart.Invoke(data);
            return true;
        }

        public void NotifyAbilityEnded(Ability ability) {
            runningAbilities.Remove(ability);
        }
        
        // Apply batched effect.
        // This can be useful if e.g. an ability applies a number of effects,
        // some with instant modifiers, and some with infinite or duration modifiers.
        // By batching these effects, we can ensure that all these effect happen 
        // with reference to the same base attribute value.
        // public void ApplyBatchGameplayEffects(int sourceId, IEnumerable<(Effect Effect, AbilitySystem Target, float Level)> batchedGameplayEffects) {

        //     foreach(var effect in batchedGameplayEffects) {
        //         ApplyEffectToTarget(sourceId, effect.Effect, effect.Target, effect.Level);
        //     }
            // var instantEffects = batchedGameplayEffects.Where(x => x.Effect.Policy.DurationPolicy == DurationPolicy.Instant);
            // var durationalEffects = batchedGameplayEffects.Where(
            //     x =>
            //         x.Effect.Policy.DurationPolicy == DurationPolicy.Duration ||
            //         x.Effect.Policy.DurationPolicy == DurationPolicy.Infinite
            //         );
            // Apply instant effects
            // foreach (var effect in instantEffects) {
            //     ApplyEffectToTarget(effect.Effect, effect.Target);
            // }
            // Apply durational effects
            // foreach (var effect in durationalEffects) {
            //     ApplyEffectToTarget(effect.Effect, effect.Target);
            // }
        // }
        
        public void ApplyEffectToTarget(int sourceId, Effect effect, AbilitySystem target, float level = 0) 
        {
            foreach(ModifierConfig modifiers in effect.Configs.Modifiers) {
                if (!target.IsAttributeExist(modifiers.AttributeType)) {
                    Debug.Log($"Being modified attribute {modifiers.AttributeType} doesn't exist in abilitySystem {target.name}. ", this);
                    return ;
                }
            }

            if (effect.Configs.DurationConfig.Policy == DurationPolicy.Instant) {
                effect.InstantApplyTo(target);
            } else {
                EffectContext effectContext = new EffectContext(sourceId, effect, this, target);
                target.ActivedEffects.ApplyDurationalEffect(effectContext);
            }

            // remove effects that mark remove from config
            List<RemoveEffectInfo> beRemovedInfo = effect.Configs.RemoveEffectsInfo;
            var beRemovedEffects = target.GetAllDurationalEffects()
                                .Where(x => beRemovedInfo.Any(y => x.Effect.Configs.Id == y.RemoveId))
                                .Join(beRemovedInfo, x => x.Effect.Configs.Id, y => y.RemoveId, (x, y) => 
                                            new { Id = x.Effect.Configs.Id, EffectContext = x, Stacks = y.RemoveStacks })
                                .OrderBy(x => x.EffectContext.RemainingDuration);

            Dictionary<Effect, int> stacks = new Dictionary<Effect, int>();
            foreach(var beRemovedEffect in beRemovedEffects) {
                Effect e = beRemovedEffect.EffectContext.Effect;
                if (!stacks.ContainsKey(e)) {
                    stacks.Add(e, 0);
                }
                if (beRemovedEffect.Stacks == 0 || stacks[e] < beRemovedEffect.Stacks ) {
                    beRemovedEffect.EffectContext.ForceEndEffect();
                }
                stacks[e]++;
            }

            EffectCues cues = effect.Configs.EffectCues;
            cues.HandleCue(target, CueEventMomentType.OnActive);
        }

        public IEnumerable<EffectContext> GetAllDurationalEffects()
        {
            List<EffectContext> durationEffects = ActivedEffects.AllEffects;
            if (durationEffects == null) {
                return new List<EffectContext>();
            }
            return durationEffects.Where(x => x.Effect.Configs.EffectType == EffectType.Normal && x.Effect.Configs.DurationConfig.Policy == DurationPolicy.Duration);
        }

        public void OnAnimationEvent(string param) => OnAnimEvent.Invoke(param);

// attribute -----------------------------------------------------------------------------
        public bool IsAttributeExist(string type) => attributeSet.Attributes.Exists( x => x.AttributeType == type);
        public void AddAttribute(string type, float baseValue, float currentValue) => attributeSet.Add(type, baseValue, currentValue);
        public float GetBaseValue(string type) => GetAttributeByType(type).BaseValue;
        public float GetCurrentValue(string type) => GetAttributeByType(type).CurrentValue;
        public void SetBaseValue(string type, float value) => GetAttributeByType(type).SetBaseValue(attributeSet, ref value);
        public void SetCurrentValue(string type, float value) => GetAttributeByType(type).SetCurrentValue(attributeSet, ref value);
        public Attribute GetAttributeByType(string type) => attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == type);

        public void ReEvaluateCurrentValueFor(string attributeType)
        {
            IEnumerable<AttributeOperationContainer> operations = ActivedEffects.GetAllOperationTo(attributeType);
            if (operations.Count() != 0) {
                ActivedEffects.UpdateAttribute(attributeType, operations);
            } else {
                SetCurrentValue(attributeType, GetBaseValue(attributeType));
            }
        }
    }
}
