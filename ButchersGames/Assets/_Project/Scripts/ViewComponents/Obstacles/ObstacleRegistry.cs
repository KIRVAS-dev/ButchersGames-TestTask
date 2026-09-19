using System.Collections.Generic;
using Core.Gameplay.Obstacle;
using ViewComponents.Level;

namespace ViewComponents.Obstacles
{
    public sealed class ObstacleRegistry : IObstacleRegistry
    {
        private readonly LevelProvider _levelProvider;

        public ObstacleRegistry(LevelProvider levelProvider)
        {
            _levelProvider = levelProvider;
        }

        public IReadOnlyCollection<IObstacle> Obstacles => _levelProvider.CurrentLevel.Obstacles;
    }
}
