using System.Collections.Generic;

namespace Core.Gameplay.Obstacle
{
    public interface IObstacleRegistry
    {
        IReadOnlyCollection<IObstacle> Obstacles { get; }
    }
}
