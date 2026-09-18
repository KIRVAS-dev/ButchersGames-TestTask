using System;
using System.Collections.Generic;

namespace Core.Gameplay.LaneBarrier
{
    public interface ILaneBarrierRegistry
    {
        IReadOnlyList<ILaneBarrier> Barriers { get; }

        event Action BarriersChanged;
    }
}
