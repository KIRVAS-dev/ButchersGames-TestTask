using Core.Gameplay.RunnerMovement;
using Core.Input.RunnerMovement;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.RunnerMovement
{
    [CreateAssetMenu(menuName = "Configs/Runner Movement")]
    public sealed class RunnerMovementConfig
        : ScriptableObject,
          IRunnerMovementSettings,
          IRunnerMovementInputSettings
    {
        [SerializeField] private float _forwardSpeed;
        [SerializeField] private float _trackHalfWidth;
        [SerializeField] private float _lateralCorrectionSpeed;
        [SerializeField] private float _lateralDragSensitivity;

        public float ForwardSpeed => _forwardSpeed;
        public float TrackHalfWidth => _trackHalfWidth;
        public float LateralDragSensitivity => _lateralDragSensitivity;
        public float LateralCorrectionSpeed => _lateralCorrectionSpeed;

        public void Validate()
        {
            Guard.AgainstNonPositive(
                _forwardSpeed,
                () => new InvalidRunnerMovementValueException(nameof(_forwardSpeed), _forwardSpeed)
            );

            Guard.AgainstNonPositive(
                _trackHalfWidth,
                () => new InvalidRunnerMovementValueException(nameof(_trackHalfWidth), _trackHalfWidth)
            );

            Guard.AgainstNonPositive(
                _lateralCorrectionSpeed,
                () => new InvalidRunnerMovementValueException(nameof(_lateralCorrectionSpeed), _lateralCorrectionSpeed)
            );

            Guard.AgainstNonPositive(
                _lateralDragSensitivity,
                () => new InvalidRunnerMovementValueException(nameof(_lateralDragSensitivity), _lateralDragSensitivity)
            );
        }
    }
}
