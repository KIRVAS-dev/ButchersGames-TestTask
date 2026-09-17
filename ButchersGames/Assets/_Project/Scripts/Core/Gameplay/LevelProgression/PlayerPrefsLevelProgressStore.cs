using UnityEngine;

namespace Core.Gameplay.LevelProgression
{
    public sealed class PlayerPrefsLevelProgressStore : ILevelProgressStore
    {
        private const string CompletedLevelCountPrefsKey = "level-completed-count";
        private const string CurrentLevelIndexPrefsKey = "level-current-index";

        public int LoadCompletedLevelCount() => PlayerPrefs.GetInt(CompletedLevelCountPrefsKey, 0);
        public int LoadCurrentLevelIndex() => PlayerPrefs.GetInt(CurrentLevelIndexPrefsKey, 0);

        public void SaveProgress(int completedLevelCount, int currentLevelIndex)
        {
            PlayerPrefs.SetInt(CompletedLevelCountPrefsKey, completedLevelCount);
            PlayerPrefs.SetInt(CurrentLevelIndexPrefsKey, currentLevelIndex);
            PlayerPrefs.Save();
        }
    }
}
