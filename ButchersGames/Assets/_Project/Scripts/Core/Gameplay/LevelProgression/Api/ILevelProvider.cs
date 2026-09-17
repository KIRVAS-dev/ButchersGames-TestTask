using System;

namespace Core.Gameplay.LevelProgression
{
    public interface ILevelProvider
    {
        int LevelCount { get; }
        bool IsRandomized { get; }

        event Action LevelLoaded;
    }
}
