using System;
using System.Collections.Generic;

namespace Core.Gameplay.Obstacle
{
    public interface IObstacleRegistry
    {
        IReadOnlyList<IObstacle> Obstacles { get; }

        event Action ObstaclesChanged;
    }
}
