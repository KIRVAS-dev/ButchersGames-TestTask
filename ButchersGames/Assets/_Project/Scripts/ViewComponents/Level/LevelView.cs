using Core.Gameplay.LevelProgression;
using ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Level
{
    public sealed class LevelView : MonoBehaviour, ILevelView
    {
        [SerializeField] private LevelListConfig _levelListConfig;

        private void Awake()
        {
            Validate();
        }

        public void LoadLevel(int levelIndex)
        {
            Level levelPrefab = _levelListConfig.Levels[levelIndex];
            Guard.AgainstNull(levelPrefab, () => new MissingLevelPrefabException(levelIndex, gameObject.name));

            ClearChildren();
            SpawnLevel(levelPrefab);
        }

        private void Validate()
        {
            Guard.AgainstNull(
                _levelListConfig,
                () => new MissingLevelListConfigException(nameof(_levelListConfig), gameObject.name));
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
                Instantiate(levelPrefab, transform);
            }
            else
            {
                UnityEditor.PrefabUtility.InstantiatePrefab(levelPrefab, transform);
            }
#else
            Instantiate(levelPrefab, transform);
#endif
        }
    }
}
