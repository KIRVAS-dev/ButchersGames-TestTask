using System.Collections.Generic;
using Core.Gameplay.LaneBarrier;
using ViewComponents.Level;

namespace ViewComponents.LaneBarriers
{
    public sealed class LaneBarrierRegistry : ILaneBarrierRegistry
    {
        private readonly LevelProvider _levelProvider;

        public LaneBarrierRegistry(LevelProvider levelProvider)
        {
            _levelProvider = levelProvider;
        }

        public IReadOnlyCollection<ILaneBarrier> Barriers => _levelProvider.CurrentLevel.Barriers;
    }
}
