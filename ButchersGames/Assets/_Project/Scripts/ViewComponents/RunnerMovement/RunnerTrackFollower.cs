using System;
using Core;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.RunnerMovement;
using Infrastructure.ExtendedExceptions;
using R3;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using VContainer;

namespace ViewComponents.RunnerMovement
{
    public sealed class RunnerTrackFollower
        : MonoBehaviour,
          IRunnerTrackFollowerView
    {
        private const int SkipInitialValue = 1;

        private RunnerMovementState _movementState = RunnerMovementState.Moving;
        private ILevelProvider _levelProvider;
        private IGameplayInputBlock _inputBlock;
        private IRunnerMovementSettings _settings;
        private IRunnerMovementService _runnerMovementService;
        private IDisposable _inputBlockSubscription;
        private RunnerMovementView _view;
        private SplineContainer _splineContainer;
        private float _lateralOffset;
        private float _distanceTraveled;
        private float _splineLength;
        private bool _isLevelLoaded;

        [Inject]
        private void Construct(
            ILevelProvider levelProvider,
            IGameplayInputBlock inputBlock,
            IRunnerMovementSettings settings,
            IRunnerMovementService runnerMovementService,
            RunnerMovementView view)
        {
            _levelProvider = levelProvider;
            _inputBlock = inputBlock;
            _settings = settings;
            _runnerMovementService = runnerMovementService;
            _view = view;

            _levelProvider.LevelLoaded += OnLevelLoaded;

            SubscribeToInputBlockChanges();
        }

        private void Awake()
        {
            Guard.AgainstNull(_settings, () => new MissingRunnerMovementViewFieldException(nameof(_settings), gameObject.name));
            Guard.AgainstNull(_view, () => new MissingRunnerMovementViewFieldException(nameof(_view), gameObject.name));

            RefreshTrackFollowing();
        }

        private void Update()
        {
            _runnerMovementService.AdvanceLateralCorrections(Time.deltaTime);

            _distanceTraveled += _settings.ForwardSpeed * Time.deltaTime;

            float normalizedSplineProgress = Mathf.Clamp01(_distanceTraveled / _splineLength);

            _splineContainer.Evaluate(normalizedSplineProgress, out float3 position, out float3 tangent, out _);

            Vector3 forward = ((Vector3)tangent).normalized;
            bool hasMovementDirection = forward.sqrMagnitude > Mathf.Epsilon;

            Guard.AgainstTrue(!hasMovementDirection, () => new InvalidSplineTangentException(gameObject.name));

            Vector3 lateralAxis = Vector3.Cross(forward, Vector3.up).normalized;
            Vector3 worldPosition = (Vector3)position + lateralAxis * _lateralOffset;

            _view.ApplyPositionAndRotation(worldPosition, Quaternion.LookRotation(forward, Vector3.up));
        }

        private void OnDestroy()
        {
            _levelProvider.LevelLoaded -= OnLevelLoaded;
            _inputBlockSubscription?.Dispose();
        }

        public void SetLateralOffset(float value)
        {
            _lateralOffset = value;
        }

        public void SetMovementState(RunnerMovementState state)
        {
            _movementState = state;

            RefreshTrackFollowing();
        }

        private void OnLevelLoaded()
        {
            _splineContainer = FindAnyObjectByType<SplineContainer>();

            Guard.AgainstNull(_splineContainer, () => new MissingSplineContainerException(gameObject.name));

            _distanceTraveled = 0f;
            _splineLength = _splineContainer.CalculateLength();

            Guard.AgainstNonPositive(_splineLength, () => new InvalidSplineLengthException(gameObject.name, _splineLength));

            _isLevelLoaded = true;
            RefreshTrackFollowing();
        }

        private void SubscribeToInputBlockChanges()
        {
            _inputBlockSubscription = _inputBlock.IsBlocked.Skip(SkipInitialValue).Subscribe(_ => RefreshTrackFollowing());
        }

        private void RefreshTrackFollowing()
        {
            bool isReadyToFollowTrack = _isLevelLoaded
             && !_inputBlock.IsBlocked.CurrentValue
             && _movementState == RunnerMovementState.Moving;

            enabled = isReadyToFollowTrack;
        }
    }
}
