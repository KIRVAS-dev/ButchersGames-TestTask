using System;
using Core.Gameplay.RunnerMovement;
using Core.Lifecycle;
using R3;

namespace ViewComponents.RunnerMovement
{
    public sealed class RunnerMovementPresenter : ISubscriptionLifecycle
    {
        private const int SkipInitialValue = 1;

        private readonly IRunnerMovementView _view;
        private readonly IRunnerMovementEvents _movementEvents;
        private readonly IReadOnlyRunnerMovementModel _model;

        private IDisposable _coordinateSubscription;
        private IDisposable _lateralOffsetSubscription;

        public RunnerMovementPresenter(
            IRunnerMovementView view,
            IRunnerMovementEvents movementEvents,
            IReadOnlyRunnerMovementModel model)
        {
            _view = view;
            _movementEvents = movementEvents;
            _model = model;
        }

        void ISubscriptionLifecycle.Start()
        {
            _movementEvents.PositionReset += OnPositionReset;

            _coordinateSubscription = _model.CurrentRunnerCoordinate.Skip(SkipInitialValue).Subscribe(_view.SetCoordinate);
            _lateralOffsetSubscription = _model.LateralOffset.Skip(SkipInitialValue).Subscribe(_view.SetLateralOffset);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _movementEvents.PositionReset -= OnPositionReset;

            _coordinateSubscription?.Dispose();
            _lateralOffsetSubscription?.Dispose();
        }

        private void OnPositionReset()
        {
            _view.SetCoordinate(_model.CurrentRunnerCoordinate.CurrentValue);
            _view.SetLateralOffset(_model.LateralOffset.CurrentValue);
        }
    }
}
