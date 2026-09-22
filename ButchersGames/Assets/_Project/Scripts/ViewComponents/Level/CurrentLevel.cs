using System;
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
        public IReadOnlyCollection<IObstacle> Obstacles { get; private set; } = Array.Empty<IObstacle>();
        public IReadOnlyCollection<ILaneBarrier> Barriers { get; private set; } = Array.Empty<ILaneBarrier>();
        public IReadOnlyCollection<IWealthPointsModifier> Modifiers { get; private set; } = Array.Empty<IWealthPointsModifier>();
        public float StartCoordinate { get; private set; }
        public float FinishCoordinate { get; private set; }
        public float RunLength => FinishCoordinate - StartCoordinate;
        internal TrackPath Track { get; private set; }

        internal void Set(Level level)
        {
            Obstacle[] obstacles = level.GetComponentsInChildren<Obstacle>();
            LaneBarrierZone[] barriers = level.GetComponentsInChildren<LaneBarrierZone>();
            WealthPointsModifierCollider[] modifiers = level.GetComponentsInChildren<WealthPointsModifierCollider>();

            TrackPath track = new TrackPath(level.SplineContainer);

            Guard.AgainstNonPositive(track.Length, () => new InvalidSplineLengthException(level.gameObject.name, track.Length));

            float startCoordinate = track.NearestCoordinateTo(level.StartPosition);
            float finishCoordinate = track.NearestCoordinateTo(level.FinishPosition);

            Guard.AgainstNonPositive(
                finishCoordinate - startCoordinate,
                () => new InvalidLevelRunException(level.gameObject.name, startCoordinate, finishCoordinate)
            );

            foreach (LaneBarrierZone barrier in barriers)
            {
                barrier.Initialize(track);
            }

            Track = track;
            StartCoordinate = startCoordinate;
            FinishCoordinate = finishCoordinate;
            Obstacles = obstacles;
            Barriers = barriers;
            Modifiers = modifiers;
        }
    }
}
