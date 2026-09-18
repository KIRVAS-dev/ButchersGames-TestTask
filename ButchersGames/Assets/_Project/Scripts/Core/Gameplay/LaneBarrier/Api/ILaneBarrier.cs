using System;

namespace Core.Gameplay.LaneBarrier
{
    public interface ILaneBarrier
    {
        float MinLateralOffset { get; }
        float MaxLateralOffset { get; }

        event Action<ILaneBarrier> Entered;
        event Action<ILaneBarrier> Exited;
    }
}
