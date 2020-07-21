using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameplayAbilitySystem.Abilities;
using GameplayAbilitySystem.Effects;
using UnityEngine;
using GameplayAbilitySystem.Attributes;
using GameplayAbilitySystem.Cues;
using GameplayAbilitySystem.Utility;
using UnityEngine.Events;

namespace GameplayAbilitySystem 
{
    /// The AbilitySytem is the primary component. Every game object 
    /// that needs to participate with the GAS needs to have this component attached.
    [AddComponentMenu("Ability System/Ability System")]
    public class AbilitySystem : MonoBehaviour 
    {
        // 自己身上作为目标的点
        [SerializeField] private Transform targetPoint = default;
        public Transform TargetPoint => targetPoint;

        // Called when a AbilityEvent is executed
        private AbilityEvent onAbilityEvent = new AbilityEvent();
        public AbilityEvent OnAbilityStart => onAbilityEvent;
        private AnimEvent onAnimEvent = new AnimEvent();
        public AnimEvent OnAnimEvent => onAnimEvent;

        private List<Ability> runningAbilities = new List<Ability>();

        // Lists all active Effect
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

            // Checks to see if the ability can be activated
            if (runningAbilities.Contains(ability)) {
                return false;
            }
            if (!ability.IsActivatable(this)) {
                return false;
            }

            // 一个技能的生命周期是从[释放开始]到[释放结束], 之后的工作交给effect
            // 技能仅仅是一个时序流程
            runningAbilities.Add(ability);
            ability.Commit(this, target);

            var data = new AbilityEventData();
            data.abilityId = ability.Id;
            data.ability = ability;
            data.target = target;
            OnAbilityStart.Invoke(data);
            return true;
        }

        // Notifies this AbilitySystem that the specified ability has ended
        public void NotifyAbilityEnded(Ability ability) {
            runningAbilities.Remove(ability);
        }
        
        // Apply batched effect.
        // This can be useful if e.g. an ability applies a number of effects,
        // some with instant modifiers, and some with infinite or duration modifiers.
        // By batching these effects, we can ensure that all these effect happen 
        // with reference to the same base attribute value.
        public void ApplyBatchGameplayEffects(int sourceId, IEnumerable<(Effect Effect, AbilitySystem Target, float Level)> batchedGameplayEffects) {

            foreach(var effect in batchedGameplayEffects) {
                ApplyEffectToTarget(sourceId, effect.Effect, effect.Target, effect.Level);
            }
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
        }
        
        // effect也是ablity system管理
        // Apply a effect to the target
        // The overall effect may be modulated by the Level.
        // level -> maybe used to affect the "strength" of the effect
        public void ApplyEffectToTarget(int sourceId, Effect appliedEffect, AbilitySystem target, float level = 0) {
            // Check to make sure all the attributes being modified by this effect exist on the target
            foreach(var modifiers in appliedEffect.Configs.Modifiers) {
                if (!target.IsAttributeExist(modifiers.Type)) {
                    return ;
                }
            }

            // Handling Instant effects is different to handling HasDuration and Infinite effects, instant effect modify the base value
            if (appliedEffect.Configs.DurationConfig.Policy == DurationPolicy.Instant) {
                appliedEffect.ApplyInstantEffect(target);
            } else {
                // Durational effect require attention to many more things than instant effects
                // Such as stacking and effect durations
                // Durational effect modify the current value
                var effectContext = new EffectContext(sourceId, appliedEffect, this, target);
                target.ActivedEffects.TryApplyDurationalEffect(effectContext);
            }

            // Remove all effects which have tags defined as "Be Removed Effects Tags". 
            // We do this by setting the expiry time on the effect to make it end prematurely
            // This is accomplished by finding all effects which grant these tags, and then adjusting start time
            var beRemovedInfo = appliedEffect.Configs.RemoveEffectsInfo;
            var beRemovedEffects = target.GetDurationEffects()
                                .Where(x => beRemovedInfo.Any(y => x.Effect.Configs.Id == y.RemoveId))
                                .Join(beRemovedInfo, x => x.Effect.Configs.Id, y => y.RemoveId, (x, y) => new { Id = x.Effect.Configs.Id, EffectContext = x, Stacks = y.RemoveStacks })
                                .OrderBy(x => x.EffectContext.RemainingTime);

            Dictionary<Effect, int> stacks = new Dictionary<Effect, int>();

            foreach(var beRemovedEffect in beRemovedEffects) {
                var effect = beRemovedEffect.EffectContext.Effect;
                if (!stacks.ContainsKey(effect)) {
                    stacks.Add(effect, 0);
                }
                if (beRemovedEffect.Stacks == 0 || stacks[effect] < beRemovedEffect.Stacks ) {
                    beRemovedEffect.EffectContext.ForceEndEffect();
                }
                stacks[effect]++;
            }

            // Execute gameplay cue
            List<EffectCues> cues = appliedEffect.Configs.Cues;
            for (var i = 0; i < cues.Count; i++) {
                var cue = cues[i];
                cue.HandleCue(target, CueEventMomentType.OnActive);
            }
        }

        public IEnumerable<EffectContext> GetDurationEffects()
        {
            List<EffectContext> durationEffects = ActivedEffects.AllEffects;
            if (durationEffects == null) {
                return new List<EffectContext>();
            }
            return durationEffects.Where(x => x.Effect.Configs.EffectType == EffectType.Normal && x.Effect.Configs.DurationConfig.Policy == DurationPolicy.Duration);
        }

        public void OnAnimationEvent(string param)
        {
            OnAnimEvent.Invoke(param);
        }

// attribute -----------------------------------------------------------------------------
        public bool IsAttributeExist(string type) => attributeSet.Attributes.Exists( x => x.AttributeType == type);
        public void AddAttribute(string type, float baseValue, float currentValue) {
            if (attributeSet == null) {
                Debug.Log("attributeSet is null");
            }
            attributeSet.Add(type, baseValue, currentValue);
        }

        public float GetBaseValue(string type) => GetAttributeByType(type).BaseValue;
        public float GetCurrentValue(string type) => GetAttributeByType(type).CurrentValue;
        public void SetBaseValue(string type, float value) => GetAttributeByType(type).SetBaseValue(attributeSet, ref value);
        public void SetCurrentValue(string type, float value) => GetAttributeByType(type).SetCurrentValue(attributeSet, ref value);
        public Attribute GetAttributeByType(string type) => attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == type);
    }
}
