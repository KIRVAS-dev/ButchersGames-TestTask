using Core.Gameplay.GameFlow;
using Core.Gameplay.RunnerMovement;
using Core.Lifecycle;

namespace Core.Input.RunnerMovement
{
    public sealed class RunnerMovementInputHandler : ISubscriptionLifecycle
    {
        private readonly IGameplayInputBlock _inputBlock;
        private readonly IRunnerMovementService _service;
        private readonly IRunnerMovementInputSettings _settings;
        private readonly IDragInput _dragInput;

        public RunnerMovementInputHandler(
            IGameplayInputBlock inputBlock,
            IRunnerMovementService service,
            IRunnerMovementInputSettings settings,
            IDragInput dragInput)
        {
            _inputBlock = inputBlock;
            _service = service;
            _settings = settings;
            _dragInput = dragInput;
        }

        void ISubscriptionLifecycle.Start()
        {
            _dragInput.DragNormalizedDeltaChanged += OnDragNormalizedDeltaChanged;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _dragInput.DragNormalizedDeltaChanged -= OnDragNormalizedDeltaChanged;
        }

        private void OnDragNormalizedDeltaChanged(float normalizedDelta)
        {
            if (_inputBlock.IsBlocked.CurrentValue)
            {
                return;
            }

            _service.AddNormalizedLateralOffsetDelta(normalizedDelta * _settings.LateralDragSensitivity);
        }
    }
}
