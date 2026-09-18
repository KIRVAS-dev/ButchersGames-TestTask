using System;
using Core.Gameplay.WealthMeter;
using R3;

namespace ViewComponents.WealthMeter
{
    public sealed class CharacterAppearancePresenter
    {
        private readonly WealthMeterModel _model;
        private readonly ICharacterAppearanceView _view;

        private IDisposable _stageSubscription;

        public CharacterAppearancePresenter(WealthMeterModel model, ICharacterAppearanceView view)
        {
            _model = model;
            _view = view;
        }

        public void StartListening()
        {
            _stageSubscription = _model.Stage.Subscribe(_view.SetActiveStage);
        }

        public void StopListening()
        {
            _stageSubscription?.Dispose();
        }
    }
}
