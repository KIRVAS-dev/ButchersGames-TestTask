using Core.Gameplay.GameFlow;
using Core.Gameplay.LaneBarrier;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.Obstacle;
using Core.Gameplay.Track;
using Core.Lifecycle;
using Core.Loop;

namespace Core.Gameplay.RunnerMovement
{
    public sealed class RunnerMovementService
        : IRunnerMovementService,
          IGameplayTickable,
          ISubscriptionLifecycle
    {
        private readonly ILevelLoader _levelLoader;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ITrackProvider _trackProvider;
        private readonly IObstacleRegistry _obstacleRegistry;
        private readonly ILaneBarrierRegistry _laneBarrierRegistry;
        private readonly RunnerMovementSimulator _simulator;

        public RunnerMovementService(
            ILevelLoader levelLoader,
            IGameStateMachine gameStateMachine,
            IRunnerMovementSettings settings,
            ITrackProvider trackProvider,
            IObstacleRegistry obstacleRegistry,
            ILaneBarrierRegistry laneBarrierRegistry,
            RunnerMovementModel model)
        {
            _levelLoader = levelLoader;
            _gameStateMachine = gameStateMachine;
            _trackProvider = trackProvider;
            _obstacleRegistry = obstacleRegistry;
            _laneBarrierRegistry = laneBarrierRegistry;
            _simulator = new RunnerMovementSimulator(settings, model);
        }

        void IGameplayTickable.Tick(float deltaTime)
        {
            if (_gameStateMachine.State != GameState.Run)
            {
                return;
            }

            _simulator.Tick(deltaTime, _trackProvider.FinishCoordinate);
        }

        void ISubscriptionLifecycle.Start()
        {
            _levelLoader.LevelLoaded += OnLevelLoaded;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _levelLoader.LevelLoaded -= OnLevelLoaded;

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
