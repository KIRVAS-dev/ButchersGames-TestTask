using System;
using Core.Gameplay.LevelProgression;
using ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Level
{
    public sealed class LevelProvider
        : MonoBehaviour,
          ILevelProvider
    {
        [SerializeField] private LevelListConfig _levelListConfig;

        public event Action LevelLoaded;

        public int LevelCount => _levelListConfig.Levels.Count;
        public bool IsRandomized => _levelListConfig.IsRandomized;

        public void NotifyLevelLoaded()
        {
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
        }
    }
}
