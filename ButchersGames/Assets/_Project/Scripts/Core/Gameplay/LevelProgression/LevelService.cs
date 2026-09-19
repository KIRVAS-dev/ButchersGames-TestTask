using Infrastructure.ExtendedExceptions;
using System;

namespace Core.Gameplay.LevelProgression
{
    public sealed class LevelService : ILevelService
    {
        private readonly ILevelLoader _levelLoader;
        private readonly ILevelProgressStore _progressStore;
        private readonly LevelModel _model;
        private readonly Random _random = new Random();

        public LevelService(
            ILevelLoader levelLoader,
            ILevelProgressStore progressStore,
            LevelModel model)
        {
            Guard.AgainstNonPositive(levelLoader.LevelCount, () => new InvalidLevelCountException(levelLoader.LevelCount));

            _levelLoader = levelLoader;
            _progressStore = progressStore;
            _model = model;

            _model.CompletedLevelCount = _progressStore.LoadCompletedLevelCount();
            _model.CurrentLevelIndex = _progressStore.LoadCurrentLevelIndex();

            Guard.AgainstLessThan(
                _model.CurrentLevelIndex,
                0,
                () => new InvalidLevelIndexException(_model.CurrentLevelIndex, _levelLoader.LevelCount)
            );

            Guard.AgainstGreaterThan(
                _model.CurrentLevelIndex,
                _levelLoader.LevelCount - 1,
                () => new InvalidLevelIndexException(_model.CurrentLevelIndex, _levelLoader.LevelCount)
            );
        }

        public int CurrentLevelNumber => _model.CompletedLevelCount + 1;

        public void LoadCurrentLevel()
        {
            _levelLoader.LoadLevel(_model.CurrentLevelIndex);
        }

        public void LoadNextLevel()
        {
            _model.CompletedLevelCount++;
            _model.CurrentLevelIndex = NextLevelIndex();

            _progressStore.SaveProgress(_model.CompletedLevelCount, _model.CurrentLevelIndex);
            _levelLoader.LoadLevel(_model.CurrentLevelIndex);
        }

        private int NextLevelIndex()
        {
            int levelCount = _levelLoader.LevelCount;

            if (_model.CompletedLevelCount < levelCount)
            {
                return _model.CompletedLevelCount;
            }

            bool canPickRandomLevel = _levelLoader.IsRandomized && levelCount > 1;

            if (canPickRandomLevel)
            {
                return RandomLevelIndexExcludingCurrent(levelCount);
            }

            return _model.CompletedLevelCount % levelCount;
        }

        private int RandomLevelIndexExcludingCurrent(int levelCount)
        {
            int randomIndex = _random.Next(levelCount - 1);

            return randomIndex >= _model.CurrentLevelIndex
                ? randomIndex + 1
                : randomIndex;
        }
    }
}
