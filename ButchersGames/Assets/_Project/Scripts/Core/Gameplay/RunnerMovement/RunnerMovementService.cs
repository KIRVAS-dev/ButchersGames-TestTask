using System.Collections.Generic;
using Core.Gameplay.GameFlow;
using Core.Gameplay.LaneBarrier;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.Obstacle;
using Core.Gameplay.Track;

namespace Core.Gameplay.RunnerMovement
{
    public sealed class RunnerMovementService
        : IRunnerMovementService,
          IGameplayTickable
    {
        private readonly ILevelProvider _levelProvider;
        private readonly IGameFlowService _gameFlowService;
        private readonly ITrackProvider _trackProvider;
        private readonly IObstacleRegistry _obstacleRegistry;
        private readonly ILaneBarrierRegistry _laneBarrierRegistry;
        private readonly RunnerMovementSimulator _simulator;
        private readonly List<IObstacle> _subscribedObstacles = new List<IObstacle>();
        private readonly List<ILaneBarrier> _subscribedLaneBarriers = new List<ILaneBarrier>();

        public RunnerMovementService(
            ILevelProvider levelProvider,
            IGameFlowService gameFlowService,
            IRunnerMovementSettings settings,
            ITrackProvider trackProvider,
            IObstacleRegistry obstacleRegistry,
            ILaneBarrierRegistry laneBarrierRegistry,
            RunnerMovementModel model)
        {
            _levelProvider = levelProvider;
            _gameFlowService = gameFlowService;
            _trackProvider = trackProvider;
            _obstacleRegistry = obstacleRegistry;
            _laneBarrierRegistry = laneBarrierRegistry;
            _simulator = new RunnerMovementSimulator(settings, model);
        }

        void IGameplayTickable.Tick(float deltaTime)
        {
            if (_gameFlowService.State != GameFlowState.Playing)
            {
                return;
            }

            _simulator.Tick(deltaTime, _trackProvider.Length);
        }

        public void StartListening()
        {
            _levelProvider.LevelLoaded += OnLevelLoaded;
        }

        public void StopListening()
        {
            _levelProvider.LevelLoaded -= OnLevelLoaded;

            UnsubscribeAllObstacles();
            UnsubscribeAllLaneBarriers();
        }

        public void SetNormalizedLateralOffset(float normalizedOffset)
        {
            _simulator.SetNormalizedLateralOffset(normalizedOffset);
        }

        private void OnLevelLoaded()
        {
            _simulator.Reset();

            ResubscribeToObstacles();
            ResubscribeToLaneBarriers();
        }

        private void ResubscribeToObstacles()
        {
            UnsubscribeAllObstacles();

            _subscribedObstacles.AddRange(_obstacleRegistry.Obstacles);

            foreach (IObstacle obstacle in _subscribedObstacles)
            {
                obstacle.Hit += OnObstacleHit;
                obstacle.Released += OnObstacleReleased;
            }
        }

        private void UnsubscribeAllObstacles()
        {
            foreach (IObstacle obstacle in _subscribedObstacles)
            {
                obstacle.Hit -= OnObstacleHit;
                obstacle.Released -= OnObstacleReleased;
            }

            _subscribedObstacles.Clear();
        }

        private void ResubscribeToLaneBarriers()
        {
            UnsubscribeAllLaneBarriers();

            _subscribedLaneBarriers.AddRange(_laneBarrierRegistry.Barriers);

            foreach (ILaneBarrier barrier in _subscribedLaneBarriers)
            {
                barrier.Entered += OnLaneBarrierEntered;
                barrier.Exited += OnLaneBarrierExited;
            }
        }

        private void UnsubscribeAllLaneBarriers()
        {
            foreach (ILaneBarrier barrier in _subscribedLaneBarriers)
            {
                barrier.Entered -= OnLaneBarrierEntered;
                barrier.Exited -= OnLaneBarrierExited;
            }

            _subscribedLaneBarriers.Clear();
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
