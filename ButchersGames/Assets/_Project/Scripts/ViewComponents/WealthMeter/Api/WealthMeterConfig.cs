using Core.Gameplay.WealthMeter;
using Infrastructure.ExtendedExceptions;
using ContentValidation;
using UnityEngine;

namespace ViewComponents.WealthMeter
{
    [CreateAssetMenu(menuName = "Configs/Wealth Meter Config")]
    public sealed class WealthMeterConfig
        : ScriptableObject,
          IValidatable,
          IWealthMeterSettings
    {
        [SerializeField] private int _startValue;
        [SerializeField] private int _poorThreshold;
        [SerializeField] private int _casualThreshold;
        [SerializeField] private int _middleThreshold;
        [SerializeField] private int _businessThreshold;
        [SerializeField] private int _richThreshold;

        int IWealthMeterSettings.StartValue => _startValue;
        int IWealthMeterSettings.PoorThreshold => _poorThreshold;
        int IWealthMeterSettings.CasualThreshold => _casualThreshold;
        int IWealthMeterSettings.MiddleThreshold => _middleThreshold;
        int IWealthMeterSettings.BusinessThreshold => _businessThreshold;
        int IWealthMeterSettings.RichThreshold => _richThreshold;

        public void Validate()
        {
            Guard.AgainstNonPositive(_startValue, () => Invalid(nameof(_startValue), _startValue));
            Guard.AgainstNegative(_poorThreshold, () => Invalid(nameof(_poorThreshold), _poorThreshold));
            Guard.AgainstGreaterThan(_poorThreshold, _casualThreshold - 1, () => Invalid(nameof(_poorThreshold), _poorThreshold));

            Guard.AgainstGreaterThan(
                _casualThreshold,
                _middleThreshold - 1,
                () => Invalid(nameof(_casualThreshold), _casualThreshold)
            );

            Guard.AgainstGreaterThan(
                _middleThreshold,
                _businessThreshold - 1,
                () => Invalid(nameof(_middleThreshold), _businessThreshold)
            );

            Guard.AgainstGreaterThan(
                _businessThreshold,
                _richThreshold - 1,
                () => Invalid(nameof(_businessThreshold), _businessThreshold)
            );

            return;

            ExtendedException Invalid(string fieldName, int value) => new InvalidWealthMeterValueException(fieldName, value);
        }
    }
}
