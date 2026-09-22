using System;
using Core.Gameplay.WealthMeter;
using Core.Lifecycle;
using R3;

namespace ViewComponents.WealthMeter
{
    public sealed class CharacterAppearancePresenter : ISubscriptionLifecycle
    {
        private readonly WealthMeterModel _model;
        private readonly ICharacterAppearanceView _view;

        private IDisposable _stageSubscription;

        public CharacterAppearancePresenter(WealthMeterModel model, ICharacterAppearanceView view)
        {
            _model = model;
            _view = view;
        }

        void ISubscriptionLifecycle.Start()
        {
            _stageSubscription = _model.Stage.Subscribe(_view.SetActiveStage);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _stageSubscription?.Dispose();
        }
    }
}
