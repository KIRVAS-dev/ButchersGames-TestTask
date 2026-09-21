using Core.Gameplay.GameFlow;
using Infrastructure.ExtendedExceptions;

namespace ViewComponents.CharacterAnimation
{
    internal sealed class MissingCharacterAnimationViewFieldException : ExtendedException
    {
        public MissingCharacterAnimationViewFieldException(string fieldName, string objectName)
            : base("character-animation-view-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class InvalidCharacterAnimationViewValueException : ExtendedException
    {
        public InvalidCharacterAnimationViewValueException(
            string fieldName,
            string objectName,
            float value)
            : base("character-animation-view-2", $"Field '{fieldName}' has invalid value '{value}' on '{objectName}'") { }
    }

    internal sealed class DuplicateCharacterAnimationSlotException : ExtendedException
    {
        public DuplicateCharacterAnimationSlotException(CharacterAnimationSlot slot, string objectName)
            : base("character-animation-view-3", $"Duplicate character animation slot '{slot}' on '{objectName}'") { }
    }

    internal sealed class CharacterAnimationStateNameMissingException : ExtendedException
    {
        public CharacterAnimationStateNameMissingException(CharacterAnimationSlot slot, string objectName)
            : base("character-animation-view-4", $"State name is missing for slot '{slot}' on '{objectName}'") { }
    }

    internal sealed class CharacterAnimationSlotNotMappedException : ExtendedException
    {
        public CharacterAnimationSlotNotMappedException(CharacterAnimationSlot slot, string objectName)
            : base("character-animation-view-5", $"Character animation slot '{slot}' is not mapped on '{objectName}'") { }
    }

    internal sealed class UnhandledCharacterAnimationStateException : ExtendedException
    {
        public UnhandledCharacterAnimationStateException(GameState state)
            : base(
                "character-animation-presenter-1",
                $"GameState '{state}' is not handled by the character animation presenter"
            ) { }
    }
}
