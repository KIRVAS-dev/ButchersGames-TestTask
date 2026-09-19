using System;

namespace Core.Gameplay.LevelProgression
{
    public interface ILevelLoader
    {
        int LevelCount { get; }
        bool IsRandomized { get; }
        event Action LevelLoaded;
        void LoadLevel(int levelIndex);
    }
}
