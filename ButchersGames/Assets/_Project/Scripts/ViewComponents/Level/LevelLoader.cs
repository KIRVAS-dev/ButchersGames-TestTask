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
        [SerializeField] private LevelListConfig _levelListConfig;

        private CurrentLevel _currentLevel;

        public event Action LevelLoaded;

        public int LevelCount => _levelListConfig.Levels.Count;
        public bool IsRandomized => _levelListConfig.IsRandomized;

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
            Level levelPrefab = _levelListConfig.Levels[levelIndex];

            ClearChildren();
            SpawnLevel(levelPrefab);
        }

        private void ClearChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        private void SpawnLevel(Level levelPrefab)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Level level = Instantiate(levelPrefab, transform);
                FinishLoading(level);
            }
            else
            {
                UnityEditor.PrefabUtility.InstantiatePrefab(levelPrefab, transform);
            }
#else
            Level level = Instantiate(levelPrefab, transform);
            FinishLoading(level);
#endif
        }

        private void FinishLoading(Level level)
        {
            _currentLevel.Set(level);

            LevelLoaded?.Invoke();
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
