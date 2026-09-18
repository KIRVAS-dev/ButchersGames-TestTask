using System;
using System.Collections.Generic;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.Obstacle;
using UnityEngine;
using VContainer;

namespace ViewComponents.Obstacles
{
    public sealed class ObstacleRegistry
        : MonoBehaviour,
          IObstacleRegistry
    {
        private ILevelProvider _levelProvider;
        private List<IObstacle> _obstacles = new List<IObstacle>();

        public event Action ObstaclesChanged;

        public IReadOnlyList<IObstacle> Obstacles => _obstacles;

        [Inject]
        private void Construct(ILevelProvider levelProvider)
        {
            _levelProvider = levelProvider;

            _levelProvider.LevelLoaded += Rescan;
        }

        private void OnDestroy()
        {
            _levelProvider.LevelLoaded -= Rescan;
        }

        private void Rescan()
        {
            _obstacles = new List<IObstacle>(
                FindObjectsByType<Obstacle>(FindObjectsInactive.Exclude)
            );

            ObstaclesChanged?.Invoke();
        }
    }
}
