using Core.Gameplay.RunnerMovement;
using ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.RunnerMovement
{
    [CreateAssetMenu(menuName = "Configs/Runner Movement")]
    public sealed class RunnerMovementConfig
        : ScriptableObject,
          IRunnerMovementSettings
    {
        [SerializeField] private float _forwardSpeed;
        [SerializeField] private float _trackHalfWidth;

        public float ForwardSpeed => _forwardSpeed;
        public float TrackHalfWidth => _trackHalfWidth;

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
        }
    }
}
