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
            _dragInput.DragNormalizedOffsetChanged += OnDragNormalizedOffsetChanged;
        }

        public void StopListening()
        {
            _dragInput.DragNormalizedOffsetChanged -= OnDragNormalizedOffsetChanged;
        }

        private void OnDragNormalizedOffsetChanged(float normalizedOffset)
        {
            if (_inputBlock.IsBlocked.CurrentValue)
            {
                return;
            }

            _service.SetNormalizedLateralOffset(normalizedOffset);
        }
    }
}
