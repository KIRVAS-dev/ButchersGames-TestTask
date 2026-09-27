using Core.Gameplay.WealthMeter;
using Core.Lifecycle;

namespace UI.FloatingText
{
    public sealed class FloatingTextPresenter : ISubscriptionLifecycle
    {
        private readonly IFloatingTextView _view;
        private readonly IWealthMeterEvents _wealthMeterEvents;

        private int _gainTotal;
        private int _lossTotal;

        public FloatingTextPresenter(IFloatingTextView view, IWealthMeterEvents wealthMeterEvents)
        {
            _view = view;
            _wealthMeterEvents = wealthMeterEvents;
        }

        void ISubscriptionLifecycle.Start()
        {
            _wealthMeterEvents.Increased += OnMoneyIncreased;
            _wealthMeterEvents.Decreased += OnMoneyDecreased;
            _view.GainSeriesEnded += OnGainSeriesEnded;
            _view.LossSeriesEnded += OnLossSeriesEnded;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _wealthMeterEvents.Increased -= OnMoneyIncreased;
            _wealthMeterEvents.Decreased -= OnMoneyDecreased;
            _view.GainSeriesEnded -= OnGainSeriesEnded;
            _view.LossSeriesEnded -= OnLossSeriesEnded;
        }

        private void OnMoneyIncreased(int amount)
        {
            _gainTotal += amount;
            _view.ShowGain(_gainTotal);
        }

        private void OnMoneyDecreased(int amount)
        {
            _lossTotal += amount;
            _view.ShowLoss(_lossTotal);
        }

        private void OnGainSeriesEnded()
        {
            _gainTotal = 0;
        }

        private void OnLossSeriesEnded()
        {
            _lossTotal = 0;
        }
    }
}
