using Core.Gameplay.WealthMeter;
using Core.Lifecycle;

namespace UI.FloatingText
{
    public sealed class FloatingTextPresenter : ISubscriptionLifecycle
    {
        private readonly IFloatingTextView _view;
        private readonly IWealthMeterService _wealthMeterService;

        public FloatingTextPresenter(IFloatingTextView view, IWealthMeterService wealthMeterService)
        {
            _view = view;
            _wealthMeterService = wealthMeterService;
        }

        void ISubscriptionLifecycle.Start()
        {
            _wealthMeterService.Increased += OnMoneyIncreased;
            _wealthMeterService.Decreased += OnMoneyDecreased;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _wealthMeterService.Increased -= OnMoneyIncreased;
            _wealthMeterService.Decreased -= OnMoneyDecreased;
        }

        private void OnMoneyIncreased(int amount)
        {
            _view.ShowGain(amount);
        }

        private void OnMoneyDecreased(int amount)
        {
            _view.ShowLoss(amount);
        }
    }
}
