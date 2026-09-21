using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.LevelProgression
{
    internal sealed class InvalidLevelCountException : ExtendedException
    {
        public InvalidLevelCountException(int levelCount)
            : base("level-1", $"Level count from provider must be positive, got {levelCount}") { }
    }

    internal sealed class InvalidLevelIndexException : ExtendedException
    {
        public InvalidLevelIndexException(int levelIndex, int levelCount)
            : base("level-2", $"Saved level index {levelIndex} is out of range for {levelCount} levels") { }
    }
}
