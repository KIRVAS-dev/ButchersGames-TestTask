using Core.Gameplay.WealthMeter;
using Core.Lifecycle;

namespace ViewComponents.WealthMeter
{
    public sealed class WealthParticlesPresenter : ISubscriptionLifecycle
    {
        private readonly IWealthMeterEvents _wealthMeterEvents;
        private readonly IWealthParticlesView _view;

        public WealthParticlesPresenter(IWealthMeterEvents wealthMeterEvents, IWealthParticlesView view)
        {
            _wealthMeterEvents = wealthMeterEvents;
            _view = view;
        }

        void ISubscriptionLifecycle.Start()
        {
            _wealthMeterEvents.Increased += OnWealthIncreased;
            _wealthMeterEvents.Decreased += OnWealthDecreased;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _wealthMeterEvents.Increased -= OnWealthIncreased;
            _wealthMeterEvents.Decreased -= OnWealthDecreased;
        }

        private void OnWealthIncreased(int amount)
        {
            _view.PlayIncrease();
        }

        private void OnWealthDecreased(int amount)
        {
            _view.PlayDecrease();
        }
    }
}
