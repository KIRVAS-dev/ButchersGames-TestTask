using System.Collections.Generic;
using Core.Gameplay.WealthMeter;

namespace Core.Gameplay.WealthPointsModifier
{
    public sealed class WealthPointsModifierService
    {
        private readonly IWealthPointsModifierRegistry _registry;
        private readonly IWealthMeterService _wealthMeter;
        private readonly List<IWealthPointsModifier> _subscribed = new List<IWealthPointsModifier>();

        public WealthPointsModifierService(IWealthPointsModifierRegistry registry, IWealthMeterService wealthMeter)
        {
            _registry = registry;
            _wealthMeter = wealthMeter;
        }

        public void StartListening()
        {
            _registry.ModifiersChanged += ResubscribeToModifiers;

            ResubscribeToModifiers();
        }

        public void StopListening()
        {
            _registry.ModifiersChanged -= ResubscribeToModifiers;

            UnsubscribeAllModifiers();
        }

        private void ResubscribeToModifiers()
        {
            UnsubscribeAllModifiers();

            _subscribed.AddRange(_registry.Modifiers);

            foreach (IWealthPointsModifier modifier in _subscribed)
            {
                modifier.Triggered += OnTriggered;
            }
        }

        private void UnsubscribeAllModifiers()
        {
            foreach (IWealthPointsModifier modifier in _subscribed)
            {
                modifier.Triggered -= OnTriggered;
            }

            _subscribed.Clear();
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
