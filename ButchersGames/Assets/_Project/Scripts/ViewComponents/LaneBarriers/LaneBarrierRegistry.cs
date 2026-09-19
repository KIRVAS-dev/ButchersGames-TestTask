using System;
using System.Collections.Generic;
using Core.Gameplay.LaneBarrier;

namespace ViewComponents.LaneBarriers
{
    public sealed class LaneBarrierRegistry
        : SceneRegistry<LaneBarrierZone, ILaneBarrier>,
          ILaneBarrierRegistry
    {
        public event Action BarriersChanged;

        public IReadOnlyList<ILaneBarrier> Barriers => Items;

        protected override void NotifyItemsChanged()
        {
            BarriersChanged?.Invoke();
        }
    }
}
