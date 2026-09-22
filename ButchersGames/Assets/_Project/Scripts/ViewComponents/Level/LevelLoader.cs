using System;
using Core.Gameplay.LevelProgression;
using Infrastructure.ExtendedExceptions;
using UnityEngine;
using VContainer;

namespace ViewComponents.Level
{
    public sealed class LevelLoader
        : MonoBehaviour,
          ILevelLoader
    {
        [SerializeField] private LevelListConfig _config;

        private CurrentLevel _currentLevel;

        public event Action LevelLoaded;

        public int LevelCount => _config.Levels.Count;
        public bool IsRandomized => _config.IsRandomized;

        [Inject]
        private void Construct(CurrentLevel currentLevel)
        {
            _currentLevel = currentLevel;
        }

        private void Awake()
        {
            Validate();
        }

        public void LoadLevel(int levelIndex)
        {
            Level levelPrefab = _config.Levels[levelIndex];

            ClearChildren();
            SpawnLevel(levelPrefab);
        }

        private void ClearChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        private void SpawnLevel(Level levelPrefab)
        {
            Level level = Instantiate(levelPrefab, transform);
            _currentLevel.Set(level);

            LevelLoaded?.Invoke();
        }

        private void Validate()
        {
            Guard.AgainstNull(_config, () => new MissingLevelListConfigException(nameof(_config), gameObject.name));

            _config.Validate();
        }
    }
}
