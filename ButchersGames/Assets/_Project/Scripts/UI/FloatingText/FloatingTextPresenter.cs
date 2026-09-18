using Core.Gameplay.WealthMeter;

namespace UI.FloatingText
{
    public sealed class FloatingTextPresenter
    {
        private readonly IFloatingTextView _view;
        private readonly IWealthMeterService _wealthMeterService;

        public FloatingTextPresenter(IFloatingTextView view, IWealthMeterService wealthMeterService)
        {
            _view = view;
            _wealthMeterService = wealthMeterService;
        }

        public void StartListening()
        {
            _wealthMeterService.Increased += OnMoneyIncreased;
            _wealthMeterService.Decreased += OnMoneyDecreased;
        }

        public void StopListening()
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
