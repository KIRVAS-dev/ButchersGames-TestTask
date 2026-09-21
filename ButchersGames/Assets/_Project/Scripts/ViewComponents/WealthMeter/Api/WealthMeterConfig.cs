using Core.Gameplay.WealthMeter;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.WealthMeter
{
    [CreateAssetMenu(menuName = "Configs/Wealth Meter Config")]
    public sealed class WealthMeterConfig
        : ScriptableObject,
          IWealthMeterSettings
    {
        [SerializeField] private int _startValue;
        [SerializeField] private int _poorThreshold;
        [SerializeField] private int _casualThreshold;
        [SerializeField] private int _middleThreshold;
        [SerializeField] private int _businessThreshold;
        [SerializeField] private int _richThreshold;

        public int StartValue => _startValue;
        public int PoorThreshold => _poorThreshold;
        public int CasualThreshold => _casualThreshold;
        public int MiddleThreshold => _middleThreshold;
        public int BusinessThreshold => _businessThreshold;
        public int RichThreshold => _richThreshold;

        public void Validate()
        {
            Guard.AgainstNonPositive(_startValue, () => Invalid(nameof(_startValue), _startValue));
            Guard.AgainstNegative(_poorThreshold, () => Invalid(nameof(_poorThreshold), _poorThreshold));
            Guard.AgainstGreaterThan(_poorThreshold, _casualThreshold - 1, () => Invalid(nameof(_poorThreshold), _poorThreshold));
            Guard.AgainstGreaterThan(_casualThreshold, _middleThreshold - 1, () => Invalid(nameof(_casualThreshold), _casualThreshold));
            Guard.AgainstGreaterThan(_middleThreshold, _businessThreshold - 1, () => Invalid(nameof(_middleThreshold), _middleThreshold));
            Guard.AgainstGreaterThan(_businessThreshold, _richThreshold - 1, () => Invalid(nameof(_businessThreshold), _businessThreshold));

            return;

            ExtendedException Invalid(string fieldName, int value) => new InvalidWealthMeterValueException(fieldName, value);
        }
    }
}
