using System;
using Core.Gameplay.WealthPointsModifier;

namespace Core.Gameplay.Obstacle
{
    public interface IObstacle
    {
        event Action Hit;
        event Action Released;

        IWealthPointsModifier Modifier { get; }
    }
}
