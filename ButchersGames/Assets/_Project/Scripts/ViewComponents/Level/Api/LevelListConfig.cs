using System.Collections.Generic;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Level
{
    [CreateAssetMenu(menuName = "Configs/Level List")]
    public sealed class LevelListConfig : ScriptableObject
    {
        [SerializeField] private bool _isRandomized;
        [SerializeField] private List<Level> _levels;

        public bool IsRandomized => _isRandomized;
        public IReadOnlyList<Level> Levels => _levels;

        public void Validate()
        {
            Guard.AgainstNullOrEmpty(_levels, () => new EmptyLevelListException(name));

            for (int i = 0; i < _levels.Count; i++)
            {
                int levelIndex = i;

                Guard.AgainstNull(_levels[levelIndex], () => new MissingLevelPrefabException(levelIndex, name));
            }
        }
    }
}
