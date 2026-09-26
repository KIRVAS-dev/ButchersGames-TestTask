using Core.Gameplay.WealthMeter;
using Core.Lifecycle;

namespace UI.FloatingText
{
    public sealed class FloatingTextPresenter : ISubscriptionLifecycle
    {
        private readonly IFloatingTextView _view;
        private readonly IWealthMeterEvents _wealthMeterEvents;

        public FloatingTextPresenter(IFloatingTextView view, IWealthMeterEvents wealthMeterEvents)
        {
            _view = view;
            _wealthMeterEvents = wealthMeterEvents;
        }

        void ISubscriptionLifecycle.Start()
        {
            _wealthMeterEvents.Increased += OnMoneyIncreased;
            _wealthMeterEvents.Decreased += OnMoneyDecreased;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _wealthMeterEvents.Increased -= OnMoneyIncreased;
            _wealthMeterEvents.Decreased -= OnMoneyDecreased;
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
