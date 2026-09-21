using Core.Gameplay.GameFlow;
using Core.Gameplay.WealthPointsModifier;
using Infrastructure.ExtendedExceptions;

namespace ViewComponents.CharacterAnimation
{
    internal sealed class MissingCharacterAnimationViewFieldException : ExtendedException
    {
        internal MissingCharacterAnimationViewFieldException(string fieldName, string objectName)
            : base("character-animation-view-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class InvalidCharacterAnimationViewValueException : ExtendedException
    {
        internal InvalidCharacterAnimationViewValueException(
            string fieldName,
            string objectName,
            float value)
            : base("character-animation-view-2", $"Field '{fieldName}' has invalid value '{value}' on '{objectName}'") { }
    }

    internal sealed class DuplicateCharacterAnimationSlotException : ExtendedException
    {
        internal DuplicateCharacterAnimationSlotException(CharacterAnimationSlot slot, string objectName)
            : base("character-animation-view-3", $"Duplicate character animation slot '{slot}' on '{objectName}'") { }
    }

    internal sealed class CharacterAnimationStateNameMissingException : ExtendedException
    {
        internal CharacterAnimationStateNameMissingException(CharacterAnimationSlot slot, string objectName)
            : base("character-animation-view-4", $"State name is missing for slot '{slot}' on '{objectName}'") { }
    }

    internal sealed class CharacterAnimationSlotNotMappedException : ExtendedException
    {
        internal CharacterAnimationSlotNotMappedException(CharacterAnimationSlot slot, string objectName)
            : base("character-animation-view-5", $"Character animation slot '{slot}' is not mapped on '{objectName}'") { }
    }

    internal sealed class UnhandledCharacterAnimationStateException : ExtendedException
    {
        internal UnhandledCharacterAnimationStateException(GameState state)
            : base(
                "character-animation-presenter-1",
                $"GameState '{state}' is not handled by the character animation presenter"
            ) { }
    }

    internal sealed class UnhandledWealthPointsModifierTypeException : ExtendedException
    {
        internal UnhandledWealthPointsModifierTypeException(WealthPointsModifierType modifierType)
            : base(
                "character-animation-presenter-2",
                $"WealthPointsModifierType '{modifierType}' is not handled by the character animation presenter"
            ) { }
    }
}
