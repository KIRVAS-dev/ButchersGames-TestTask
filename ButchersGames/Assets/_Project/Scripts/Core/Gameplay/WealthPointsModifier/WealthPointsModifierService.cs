using Core.Gameplay.LevelProgression;
using Core.Gameplay.WealthMeter;
using Core.Lifecycle;

namespace Core.Gameplay.WealthPointsModifier
{
    public sealed class WealthPointsModifierService : ISubscriptionLifecycle
    {
        private readonly ILevelLoaderEvents _levelLoaderEvents;
        private readonly IWealthPointsModifierRegistry _registry;
        private readonly IWealthMeterService _wealthMeter;

        public WealthPointsModifierService(
            ILevelLoaderEvents levelLoaderEvents,
            IWealthPointsModifierRegistry registry,
            IWealthMeterService wealthMeter)
        {
            _levelLoaderEvents = levelLoaderEvents;
            _registry = registry;
            _wealthMeter = wealthMeter;
        }

        void ISubscriptionLifecycle.Start()
        {
            _levelLoaderEvents.LevelLoaded += OnLevelLoaded;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _levelLoaderEvents.LevelLoaded -= OnLevelLoaded;

            UnsubscribeAllModifiers();
        }

        private void OnLevelLoaded()
        {
            foreach (IWealthPointsModifier modifier in _registry.Modifiers)
            {
                modifier.Triggered += OnTriggered;
            }
        }

        private void UnsubscribeAllModifiers()
        {
            foreach (IWealthPointsModifier modifier in _registry.Modifiers)
            {
                modifier.Triggered -= OnTriggered;
            }
        }

        private void OnTriggered(WealthPointsModifierType type, int amount)
        {
            switch (type)
            {
                case WealthPointsModifierType.Increase:
                    _wealthMeter.Increase(amount);
                    break;

                case WealthPointsModifierType.Decrease:
                    _wealthMeter.Decrease(amount);
                    break;

                default:
                    throw new InvalidWealthPointsModifierTypeException(type);
            }
        }
    }
}
