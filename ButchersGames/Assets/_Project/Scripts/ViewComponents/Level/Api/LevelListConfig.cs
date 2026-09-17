using System.Collections.Generic;
using UnityEngine;

namespace ViewComponents.Level
{
    [CreateAssetMenu(menuName = "Data/Level List")]
    public sealed class LevelListConfig : ScriptableObject
    {
        [SerializeField] private bool _isRandomized;
        [SerializeField] private List<Level> _levels;

        public bool IsRandomized => _isRandomized;
        public IReadOnlyList<Level> Levels => _levels;
    }
}
