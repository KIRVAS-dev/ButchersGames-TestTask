using ExtendedExceptions;
using UnityEngine;

namespace Core.Gameplay.LevelProgression
{
    public sealed class LevelService : ILevelService
    {
        private readonly ILevelProvider _levelProvider;
        private readonly ILevelView _levelView;
        private readonly ILevelProgressStore _progressStore;
        private readonly LevelModel _model;

        public LevelService(
            ILevelProvider levelProvider,
            ILevelView levelView,
            ILevelProgressStore progressStore,
            LevelModel model)
        {
            Guard.AgainstNonPositive(levelProvider.LevelCount, () => new InvalidLevelCountException(levelProvider.LevelCount));

            _levelProvider = levelProvider;
            _levelView = levelView;
            _progressStore = progressStore;
            _model = model;

            _model.CompletedLevelCount = _progressStore.LoadCompletedLevelCount();
            _model.CurrentLevelIndex = _progressStore.LoadCurrentLevelIndex();
        }

        public int CurrentLevelNumber => _model.CompletedLevelCount + 1;
        public int CompletedLevelCount => _model.CompletedLevelCount;

        public void SelectCurrentLevel()
        {
            _levelView.LoadLevel(_model.CurrentLevelIndex);
        }

        public void RestartLevel()
        {
            SelectCurrentLevel();
        }

        public void ProceedToNextLevel()
        {
            _model.CompletedLevelCount++;
            _model.CurrentLevelIndex = NextLevelIndex();

            _progressStore.SaveProgress(_model.CompletedLevelCount, _model.CurrentLevelIndex);
            _levelView.LoadLevel(_model.CurrentLevelIndex);
        }

        private int NextLevelIndex()
        {
            int levelCount = _levelProvider.LevelCount;

            if (_model.CompletedLevelCount < levelCount)
            {
                return _model.CompletedLevelCount;
            }

            bool canPickRandomLevel = _levelProvider.IsRandomized && levelCount > 1;

            if (canPickRandomLevel)
            {
                return RandomLevelIndexExcludingCurrent(levelCount);
            }

            return _model.CompletedLevelCount % levelCount;
        }

        private int RandomLevelIndexExcludingCurrent(int levelCount)
        {
            int randomIndex = Random.Range(0, levelCount - 1);

            return randomIndex >= _model.CurrentLevelIndex
                ? randomIndex + 1
                : randomIndex;
        }
    }
}
