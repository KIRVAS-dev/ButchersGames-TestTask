using System;
using Core.Gameplay.LevelProgression;
using Core.Gameplay.WealthMeter;
using Core.Lifecycle;

namespace Core.Gameplay.WealthPointsModifier
{
    public sealed class WealthPointsModifierService : ISubscriptionLifecycle
    {
        private const double IncreaseChance = 0.5;

        private readonly ILevelLoaderEvents _levelLoaderEvents;
        private readonly IWealthPointsModifierRegistry _registry;
        private readonly IWealthMeterService _wealthMeter;
        private readonly Random _random = new Random();

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

                case WealthPointsModifierType.Random:
                    ApplyRandomModifier(amount);
                    break;

                default:
                    throw new InvalidWealthPointsModifierTypeException(type);
            }
        }

        private void ApplyRandomModifier(int amount)
        {
            bool isIncrease = _random.NextDouble() < IncreaseChance;

            if (isIncrease)
            {
                _wealthMeter.Increase(amount);
            }
            else
            {
                _wealthMeter.Decrease(amount);
            }
        }
    }
}
