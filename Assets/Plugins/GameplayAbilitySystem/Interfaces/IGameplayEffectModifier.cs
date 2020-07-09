using AbilitySystem.Attributes;
using AbilitySystem.GameplayEffects;

namespace AbilitySystem.Interfaces {

    /// <summary>
    /// This class defines how a  <see cref="GameplayEffect"/> modifies attributes (e.g. doing damage, healing)
    /// </summary>
    public interface IGameplayEffectModifier {
        /// <summary>
        /// The attribute to modify
        /// </summary>
        /// <value></value>
        AttributeType Attribute { get; }

        /// <summary>
        /// The operation (add, multiply, etc.) to apply to the attribute
        /// </summary>
        /// <value></value>
        ModifierOperationType ModifierOperation { get; }

        /// <summary>
        /// How the modification value is obtained
        /// </summary>
        /// <value></value>
        ModifierCalculationType ModifierCalculationType { get; }

        /// <summary>
        /// Modification value for ScalableFloat type <see cref="Enums.ModifierCalculationType"/>
        /// </summary>
        /// <value></value>
        float ScaledMagnitude { get; }

        /// <summary>
        /// <see cref="GameplayTag"/> that must be present/must not be present on the source of the <see cref="AbilitySystemComponent"/>
        /// </summary>
        /// <value></value>
        GameplayEffectModifierTagCollection SourceTags { get; }

        /// <see cref="GameplayTag"/> that must be present/must not be present on the target of the <see cref="AbilitySystemComponent"/>
        GameplayEffectModifierTagCollection TargetTags { get; }

        bool AttemptCalculateMagnitude(out float evaluatedMagnitude);
    }
}
