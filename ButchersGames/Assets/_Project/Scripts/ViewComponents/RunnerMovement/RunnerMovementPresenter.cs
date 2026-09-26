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
        private readonly IRunnerMovementService _movementService;
        private readonly IReadOnlyRunnerMovementModel _model;

        private IDisposable _coordinateSubscription;
        private IDisposable _lateralOffsetSubscription;

        public RunnerMovementPresenter(
            IRunnerMovementView view,
            IRunnerMovementService movementService,
            IReadOnlyRunnerMovementModel model)
        {
            _view = view;
            _movementService = movementService;
            _model = model;
        }

        void ISubscriptionLifecycle.Start()
        {
            _movementService.PositionReset += OnPositionReset;

            _coordinateSubscription = _model.CurrentRunnerCoordinate.Skip(SkipInitialValue).Subscribe(_view.SetCoordinate);
            _lateralOffsetSubscription = _model.LateralOffset.Skip(SkipInitialValue).Subscribe(_view.SetLateralOffset);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _movementService.PositionReset -= OnPositionReset;

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
