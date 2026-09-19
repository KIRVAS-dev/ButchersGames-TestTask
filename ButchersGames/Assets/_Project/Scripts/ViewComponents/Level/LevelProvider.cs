using System;
using Core.Gameplay.LevelProgression;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Level
{
    public sealed class LevelProvider
        : MonoBehaviour,
          ILevelProvider
    {
        [SerializeField] private LevelListConfig _levelListConfig;

        private Level _currentLevel;

        public event Action LevelLoaded;

        public Level CurrentLevel
        {
            get
            {
                Guard.AgainstNull(_currentLevel, () => new LevelNotLoadedException(gameObject.name));

                return _currentLevel;
            }
        }

        public int LevelCount => _levelListConfig.Levels.Count;
        public bool IsRandomized => _levelListConfig.IsRandomized;

        public Level LevelAt(int levelIndex)
        {
            return _levelListConfig.Levels[levelIndex];
        }

        public void NotifyLevelLoaded(Level level)
        {
            _currentLevel = level;

            LevelLoaded?.Invoke();
        }

        private void Awake()
        {
            Validate();
        }

        private void Validate()
        {
            Guard.AgainstNull(
                _levelListConfig,
                () => new MissingLevelListConfigException(nameof(_levelListConfig), gameObject.name)
            );

            _levelListConfig.Validate();
        }
    }
}
