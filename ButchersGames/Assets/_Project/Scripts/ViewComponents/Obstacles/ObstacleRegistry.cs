using System;
using System.Collections.Generic;
using Core.Gameplay.Obstacle;

namespace ViewComponents.Obstacles
{
    public sealed class ObstacleRegistry
        : SceneRegistry<Obstacle, IObstacle>,
          IObstacleRegistry
    {
        public event Action ObstaclesChanged;

        public IReadOnlyList<IObstacle> Obstacles => Items;

        protected override void NotifyItemsChanged()
        {
            ObstaclesChanged?.Invoke();
        }
    }
}
