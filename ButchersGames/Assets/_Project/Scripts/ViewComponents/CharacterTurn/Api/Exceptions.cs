using Core.Gameplay.RunnerMovement;
using Infrastructure.ExtendedExceptions;

namespace ViewComponents.CharacterTurn
{
    internal sealed class InvalidCharacterTurnValueException : ExtendedException
    {
        public InvalidCharacterTurnValueException(string fieldName, float value)
            : base("character-turn-1", $"CharacterTurnConfig field '{fieldName}' has invalid value {value}") { }
    }

    internal sealed class MissingCharacterTurnConfigException : ExtendedException
    {
        public MissingCharacterTurnConfigException(string fieldName, string objectName)
            : base("character-turn-2", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class UnhandledCharacterTurnSideException : ExtendedException
    {
        public UnhandledCharacterTurnSideException(CharacterTurnSide side)
            : base("character-turn-3", $"CharacterTurnSide '{side}' is not handled by the turn animator") { }
    }

    internal sealed class UnhandledCharacterTurnDirectionException : ExtendedException
    {
        public UnhandledCharacterTurnDirectionException(RunnerLateralDirection direction)
            : base("character-turn-4", $"RunnerLateralDirection '{direction}' is not handled by the character turn presenter") { }
    }
}
