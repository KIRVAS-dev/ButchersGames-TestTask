using System;
using Core.Gameplay.RunnerMovement;
using R3;

namespace ViewComponents.RunnerMovement
{
    public sealed class RunnerMovementPresenter
    {
        private readonly RunnerMovementModel _model;
        private readonly IRunnerTrackFollowerView _view;

        private IDisposable _lateralOffsetSubscription;
        private IDisposable _movementStateSubscription;

        public RunnerMovementPresenter(RunnerMovementModel model, IRunnerTrackFollowerView view)
        {
            _model = model;
            _view = view;
        }

        public void StartListening()
        {
            _lateralOffsetSubscription = _model.LateralOffset.Subscribe(_view.SetLateralOffset);
            _movementStateSubscription = _model.State.Subscribe(_view.SetMovementState);
        }

        public void StopListening()
        {
            _lateralOffsetSubscription?.Dispose();
            _movementStateSubscription?.Dispose();
        }
    }
}
