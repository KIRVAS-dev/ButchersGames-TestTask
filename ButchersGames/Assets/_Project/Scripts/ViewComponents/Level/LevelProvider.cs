using Core.Gameplay.LevelProgression;
using ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Level
{
    public sealed class LevelProvider : MonoBehaviour, ILevelProvider
    {
        [SerializeField] private LevelListConfig _levelListConfig;

        public int LevelCount => _levelListConfig.Levels.Count;
        public bool IsRandomized => _levelListConfig.IsRandomized;

        private void Awake()
        {
            Validate();
        }

        private void Validate()
        {
            Guard.AgainstNull(
                _levelListConfig,
                () => new MissingLevelListConfigException(nameof(_levelListConfig), gameObject.name));
        }
    }
}
