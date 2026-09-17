using ExtendedExceptions;

namespace Core.Gameplay.LevelProgression
{
    public sealed class InvalidLevelCountException : ExtendedException
    {
        public InvalidLevelCountException(int levelCount)
            : base("level-1", $"Level count from provider must be positive, got {levelCount}") { }
    }
}
