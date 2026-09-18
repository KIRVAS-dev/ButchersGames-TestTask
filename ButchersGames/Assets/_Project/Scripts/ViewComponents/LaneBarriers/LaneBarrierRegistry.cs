using System;
using System.Collections.Generic;
using Core.Gameplay.LaneBarrier;
using Core.Gameplay.LevelProgression;
using UnityEngine;
using VContainer;

namespace ViewComponents.LaneBarriers
{
    public sealed class LaneBarrierRegistry
        : MonoBehaviour,
          ILaneBarrierRegistry
    {
        private ILevelProvider _levelProvider;
        private List<ILaneBarrier> _barriers = new List<ILaneBarrier>();

        public event Action BarriersChanged;

        public IReadOnlyList<ILaneBarrier> Barriers => _barriers;

        [Inject]
        private void Construct(ILevelProvider levelProvider)
        {
            _levelProvider = levelProvider;

            _levelProvider.LevelLoaded += Rescan;
        }

        private void OnDestroy()
        {
            _levelProvider.LevelLoaded -= Rescan;
        }

        private void Rescan()
        {
            _barriers = new List<ILaneBarrier>(
                FindObjectsByType<LaneBarrierZone>(FindObjectsInactive.Exclude)
            );

            BarriersChanged?.Invoke();
        }
    }
}
