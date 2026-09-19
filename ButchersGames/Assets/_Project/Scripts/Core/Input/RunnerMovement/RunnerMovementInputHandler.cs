using Core.Gameplay.GameFlow;
using Core.Gameplay.RunnerMovement;
using IDragInput = Input.IDragInput;

namespace Core.Input.RunnerMovement
{
    public sealed class RunnerMovementInputHandler
    {
        private readonly IGameplayInputBlock _inputBlock;
        private readonly IRunnerMovementService _service;
        private readonly IDragInput _dragInput;

        public RunnerMovementInputHandler(
            IGameplayInputBlock inputBlock,
            IRunnerMovementService service,
            IDragInput dragInput)
        {
            _inputBlock = inputBlock;
            _service = service;
            _dragInput = dragInput;
        }

        public void StartListening()
        {
            _dragInput.DragNormalizedDeltaChanged += OnDragNormalizedDeltaChanged;
        }

        public void StopListening()
        {
            _dragInput.DragNormalizedDeltaChanged -= OnDragNormalizedDeltaChanged;
        }

        private void OnDragNormalizedDeltaChanged(float normalizedDelta)
        {
            if (_inputBlock.IsBlocked.CurrentValue)
            {
                return;
            }

            _service.AddNormalizedLateralOffsetDelta(normalizedDelta);
        }
    }
}
