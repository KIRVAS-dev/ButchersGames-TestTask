using Core.Gameplay.RunnerMovement;
using Core.Input.RunnerMovement;
using Infrastructure.ExtendedExceptions;
using ContentValidation;
using UnityEngine;

namespace ViewComponents.RunnerMovement
{
    [CreateAssetMenu(menuName = "Configs/Runner Movement Config")]
    public sealed class RunnerMovementConfig
        : ScriptableObject,
          IValidatable,
          IRunnerMovementSettings,
          IRunnerMovementInputSettings
    {
        [SerializeField] private float _forwardSpeed;
        [SerializeField] private float _lateralRange;
        [SerializeField] private float _lateralCorrectionSpeed;
        [SerializeField] private float _lateralDragSensitivity;

        float IRunnerMovementSettings.ForwardSpeed => _forwardSpeed;
        float IRunnerMovementSettings.LateralRange => _lateralRange;
        float IRunnerMovementSettings.LateralCorrectionSpeed => _lateralCorrectionSpeed;
        float IRunnerMovementInputSettings.LateralDragSensitivity => _lateralDragSensitivity;

        public void Validate()
        {
            Guard.AgainstNonPositive(
                _forwardSpeed,
                () => new InvalidRunnerMovementValueException(nameof(_forwardSpeed), _forwardSpeed)
            );

            Guard.AgainstNonPositive(
                _lateralRange,
                () => new InvalidRunnerMovementValueException(nameof(_lateralRange), _lateralRange)
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
