using Core.Gameplay.RunnerMovement;
using IDragInput = global::Input.IDragInput;
using ITickInput = global::Input.ITickInput;

namespace Core.Input.RunnerMovement
{
    public sealed class RunnerMovementInputHandler
    {
        private readonly IGameplayInputBlock _inputBlock;
        private readonly IRunnerMovementService _service;
        private readonly IDragInput _dragInput;
        private readonly ITickInput _tickInput;

        public RunnerMovementInputHandler(
            IGameplayInputBlock inputBlock,
            IRunnerMovementService service,
            IDragInput dragInput,
            ITickInput tickInput)
        {
            _inputBlock = inputBlock;
            _service = service;
            _dragInput = dragInput;
            _tickInput = tickInput;
        }

        public void StartListening()
        {
            _dragInput.DragNormalizedOffsetChanged += OnDragNormalizedOffsetChanged;
            _tickInput.Ticked += OnTicked;
        }

        public void StopListening()
        {
            _dragInput.DragNormalizedOffsetChanged -= OnDragNormalizedOffsetChanged;
            _tickInput.Ticked -= OnTicked;
        }

        private void OnDragNormalizedOffsetChanged(float normalizedOffset)
        {
            if (_inputBlock.IsBlocked.CurrentValue)
            {
                return;
            }

            _service.SetNormalizedLateralOffset(normalizedOffset);
        }

        private void OnTicked(float deltaTime)
        {
            if (_inputBlock.IsBlocked.CurrentValue)
            {
                return;
            }

            _service.AdvanceLateralCorrections(deltaTime);
        }
    }
}
