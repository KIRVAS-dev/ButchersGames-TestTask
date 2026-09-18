using System;

namespace Core.Gameplay.Obstacle
{
    public interface IObstacle
    {
        event Action Hit;
        event Action Released;
    }
}
