using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.LaneBarrier;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.Obstacle;
using Core.Gameplay.RunnerBody;
using Core.Gameplay.Track;
using Core.Lifecycle;
using Core.Loop;

namespace Core.Gameplay.RunnerMovement
{
    public sealed class RunnerMovementService
        : IRunnerMovementService,
          IRunnerMovementEvents,
          IGameplayTickable,
          ISubscriptionLifecycle
    {
        private readonly ILevelLoaderEvents _levelLoaderEvents;
        private readonly IReadOnlyGameStateModel _gameStateModel;
        private readonly ITrackProvider _trackProvider;
        private readonly IObstacleRegistry _obstacleRegistry;
        private readonly ILaneBarrierRegistry _laneBarrierRegistry;
        private readonly RunnerMovementSimulator _simulator;

        public event Action PositionReset;

        public RunnerMovementService(
            ILevelLoaderEvents levelLoaderEvents,
            IReadOnlyGameStateModel gameStateModel,
            IRunnerMovementSettings settings,
            ITrackProvider trackProvider,
            IObstacleRegistry obstacleRegistry,
            ILaneBarrierRegistry laneBarrierRegistry,
            IRunnerBodyProvider runnerBodyProvider,
            RunnerMovementModel model)
        {
            _levelLoaderEvents = levelLoaderEvents;
            _gameStateModel = gameStateModel;
            _trackProvider = trackProvider;
            _obstacleRegistry = obstacleRegistry;
            _laneBarrierRegistry = laneBarrierRegistry;
            _simulator = new RunnerMovementSimulator(settings, runnerBodyProvider, model);
        }

        void IGameplayTickable.Tick(float deltaTime)
        {
            if (_gameStateModel.State.CurrentValue != GameState.Run)
            {
                return;
            }

            _simulator.Tick(deltaTime, _trackProvider.FinishCoordinate);
        }

        void ISubscriptionLifecycle.Start()
        {
            _levelLoaderEvents.LevelLoaded += OnLevelLoaded;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _levelLoaderEvents.LevelLoaded -= OnLevelLoaded;

            UnsubscribeAllObstacles();
            UnsubscribeAllLaneBarriers();
        }

        void IRunnerMovementService.AddNormalizedLateralOffsetDelta(float normalizedDelta)
        {
            _simulator.AddNormalizedLateralOffsetDelta(normalizedDelta);
        }

        private void OnLevelLoaded()
        {
            _simulator.Reset(_trackProvider.StartCoordinate);
            PositionReset?.Invoke();

            SubscribeToObstacles();
            SubscribeToLaneBarriers();
        }

        private void SubscribeToObstacles()
        {
            foreach (IObstacle obstacle in _obstacleRegistry.Obstacles)
            {
                obstacle.Hit += OnObstacleHit;
                obstacle.Released += OnObstacleReleased;
            }
        }

        private void UnsubscribeAllObstacles()
        {
            foreach (IObstacle obstacle in _obstacleRegistry.Obstacles)
            {
                obstacle.Hit -= OnObstacleHit;
                obstacle.Released -= OnObstacleReleased;
            }
        }

        private void SubscribeToLaneBarriers()
        {
            foreach (ILaneBarrier barrier in _laneBarrierRegistry.Barriers)
            {
                barrier.Entered += OnLaneBarrierEntered;
                barrier.Exited += OnLaneBarrierExited;
            }
        }

        private void UnsubscribeAllLaneBarriers()
        {
            foreach (ILaneBarrier barrier in _laneBarrierRegistry.Barriers)
            {
                barrier.Entered -= OnLaneBarrierEntered;
                barrier.Exited -= OnLaneBarrierExited;
            }
        }

        private void OnObstacleHit()
        {
            _simulator.HitObstacle();
        }

        private void OnObstacleReleased()
        {
            _simulator.ReleaseObstacle();
        }

        private void OnLaneBarrierEntered(ILaneBarrier barrier)
        {
            _simulator.EnterLaneBarrier(barrier);
        }

        private void OnLaneBarrierExited(ILaneBarrier barrier)
        {
            _simulator.ExitLaneBarrier(barrier);
        }
    }
}
