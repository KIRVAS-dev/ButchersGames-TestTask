using Core.Gameplay.LevelProgression;
using Core.Gameplay.WealthMeter;

namespace Core.Gameplay.WealthPointsModifier
{
    public sealed class WealthPointsModifierService
    {
        private readonly ILevelLoader _levelLoader;
        private readonly IWealthPointsModifierRegistry _registry;
        private readonly IWealthMeterService _wealthMeter;

        public WealthPointsModifierService(
            ILevelLoader levelLoader,
            IWealthPointsModifierRegistry registry,
            IWealthMeterService wealthMeter)
        {
            _levelLoader = levelLoader;
            _registry = registry;
            _wealthMeter = wealthMeter;
        }

        public void StartListening()
        {
            _levelLoader.LevelLoaded += OnLevelLoaded;
        }

        public void StopListening()
        {
            _levelLoader.LevelLoaded -= OnLevelLoaded;

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
