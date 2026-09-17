using Core.Gameplay.LevelProgression;
using Core.Gameplay.RunnerMovement;
using ExtendedExceptions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using VContainer;

namespace ViewComponents.RunnerMovement
{
    public sealed class RunnerTrackFollower : MonoBehaviour
    {
        private ILevelProvider _levelProvider;
        private IRunnerMovementSettings _settings;
        private RunnerMovementModel _model;
        private RunnerMovementView _view;
        private SplineContainer _splineContainer;
        private float _distanceTraveled;
        private float _splineLength;

        [Inject]
        private void Construct(
            ILevelProvider levelProvider,
            IRunnerMovementSettings settings,
            RunnerMovementModel model,
            RunnerMovementView view)
        {
            _levelProvider = levelProvider;
            _settings = settings;
            _model = model;
            _view = view;

            _levelProvider.LevelLoaded += OnLevelLoaded;
        }

        private void Awake()
        {
            Guard.AgainstNull(_settings, () => new MissingRunnerMovementViewFieldException(nameof(_settings), gameObject.name));
            Guard.AgainstNull(_model, () => new MissingRunnerMovementViewFieldException(nameof(_model), gameObject.name));
            Guard.AgainstNull(_view, () => new MissingRunnerMovementViewFieldException(nameof(_view), gameObject.name));

            DisableTrackFollowing();
        }

        private void Update()
        {
            _distanceTraveled += _settings.ForwardSpeed * Time.deltaTime;

            float normalizedSplineProgress = Mathf.Clamp01(_distanceTraveled / _splineLength);

            _splineContainer.Evaluate(normalizedSplineProgress, out float3 position, out float3 tangent, out _);

            Vector3 forward = ((Vector3)tangent).normalized;
            bool hasMovementDirection = forward.sqrMagnitude > Mathf.Epsilon;

            Guard.AgainstTrue(!hasMovementDirection, () => new InvalidSplineTangentException(gameObject.name));

            Vector3 lateralAxis = Vector3.Cross(forward, Vector3.up).normalized;
            Vector3 worldPosition = (Vector3)position + lateralAxis * _model.LateralOffset;

            _view.ApplyPositionAndRotation(worldPosition, Quaternion.LookRotation(forward, Vector3.up));
        }

        private void OnDestroy()
        {
            _levelProvider.LevelLoaded -= OnLevelLoaded;
        }

        private void OnLevelLoaded()
        {
            _splineContainer = FindAnyObjectByType<SplineContainer>();

            Guard.AgainstNull(_splineContainer, () => new MissingSplineContainerException(gameObject.name));

            _distanceTraveled = 0f;
            _splineLength = _splineContainer.CalculateLength();

            Guard.AgainstNonPositive(_splineLength, () => new InvalidSplineLengthException(gameObject.name, _splineLength));

            EnableTrackFollowing();
        }

        private void EnableTrackFollowing()
        {
            enabled = true;
        }

        private void DisableTrackFollowing()
        {
            enabled = false;
        }
    }
}
