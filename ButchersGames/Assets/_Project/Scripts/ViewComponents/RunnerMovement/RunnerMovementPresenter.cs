using System;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.RunnerMovement;
using R3;

namespace ViewComponents.RunnerMovement
{
    public sealed class RunnerMovementPresenter
    {
        private const int SkipInitialValue = 1;

        private readonly ILevelLoader _levelLoader;
        private readonly IRunnerMovementView _view;
        private readonly RunnerMovementModel _model;

        private IDisposable _coordinateSubscription;
        private IDisposable _lateralOffsetSubscription;

        public RunnerMovementPresenter(
            ILevelLoader levelLoader,
            IRunnerMovementView view,
            RunnerMovementModel model)
        {
            _levelLoader = levelLoader;
            _view = view;
            _model = model;
        }

        public void StartListening()
        {
            _levelLoader.LevelLoaded += OnLevelLoaded;

            _coordinateSubscription = _model.CurrentRunnerCoordinate.Skip(SkipInitialValue).Subscribe(_view.SetCoordinate);
            _lateralOffsetSubscription = _model.LateralOffset.Skip(SkipInitialValue).Subscribe(_view.SetLateralOffset);
        }

        public void StopListening()
        {
            _levelLoader.LevelLoaded -= OnLevelLoaded;

            _coordinateSubscription?.Dispose();
            _lateralOffsetSubscription?.Dispose();
        }

        private void OnLevelLoaded()
        {
            _view.SetCoordinate(_model.CurrentRunnerCoordinate.CurrentValue);
            _view.SetLateralOffset(_model.LateralOffset.CurrentValue);
        }
    }
}
