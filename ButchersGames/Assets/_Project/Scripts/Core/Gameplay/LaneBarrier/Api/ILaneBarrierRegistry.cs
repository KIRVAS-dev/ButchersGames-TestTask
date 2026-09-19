using System.Collections.Generic;

namespace Core.Gameplay.LaneBarrier
{
    public interface ILaneBarrierRegistry
    {
        IReadOnlyCollection<ILaneBarrier> Barriers { get; }
    }
}
