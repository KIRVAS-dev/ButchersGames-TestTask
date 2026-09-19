using System.Collections.Generic;
using Core.Gameplay.LaneBarrier;
using Core.Gameplay.Obstacle;
using Core.Gameplay.Track;
using Core.Gameplay.WealthPointsModifier;
using Infrastructure.ExtendedExceptions;
using ViewComponents.LaneBarriers;
using ViewComponents.Obstacles;
using ViewComponents.Track;
using ViewComponents.WealthPointsModifier;

namespace ViewComponents.Level
{
    public sealed class CurrentLevel
        : IObstacleRegistry,
          ILaneBarrierRegistry,
          IWealthPointsModifierRegistry,
          ITrackProvider
    {
        public IReadOnlyCollection<IObstacle> Obstacles { get; private set; }
        public IReadOnlyCollection<ILaneBarrier> Barriers { get; private set; }
        public IReadOnlyCollection<IWealthPointsModifier> Modifiers { get; private set; }
        public TrackPath Track { get; private set; }
        public float FinishDistance { get; private set; }
        public float Length => Track.Length;

        internal void Set(Level level)
        {
            Obstacle[] obstacles = level.GetComponentsInChildren<Obstacle>();
            LaneBarrierZone[] barriers = level.GetComponentsInChildren<LaneBarrierZone>();
            WealthPointsModifierCollider[] modifiers = level.GetComponentsInChildren<WealthPointsModifierCollider>();

            TrackPath track = new TrackPath(level.SplineContainer);

            Guard.AgainstNonPositive(track.Length, () => new InvalidSplineLengthException(level.gameObject.name, track.Length));

            foreach (LaneBarrierZone barrier in barriers)
            {
                barrier.Initialize(track);
            }

            Track = track;
            FinishDistance = track.NearestDistanceTo(level.FinishPosition);
            Obstacles = obstacles;
            Barriers = barriers;
            Modifiers = modifiers;
        }
    }
}
