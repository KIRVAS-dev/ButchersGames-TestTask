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
        [SerializeField] private int _descentThreshold;
        [SerializeField] private int _casualThreshold;
        [SerializeField] private int _richThreshold;
        [SerializeField] private int _millionaireThreshold;

        public int StartValue => _startValue;
        public int PoorThreshold => _poorThreshold;
        public int DescentThreshold => _descentThreshold;
        public int CasualThreshold => _casualThreshold;
        public int RichThreshold => _richThreshold;
        public int MillionaireThreshold => _millionaireThreshold;

        public void Validate()
        {
            Guard.AgainstNonPositive(_startValue, () => Invalid(nameof(_startValue), _startValue));
            Guard.AgainstNegative(_poorThreshold, () => Invalid(nameof(_poorThreshold), _poorThreshold));
            Guard.AgainstGreaterThan(_poorThreshold, _descentThreshold - 1, () => Invalid(nameof(_poorThreshold), _poorThreshold));
            Guard.AgainstGreaterThan(_descentThreshold, _casualThreshold - 1, () => Invalid(nameof(_descentThreshold), _descentThreshold));
            Guard.AgainstGreaterThan(_casualThreshold, _richThreshold - 1, () => Invalid(nameof(_casualThreshold), _casualThreshold));
            Guard.AgainstGreaterThan(_richThreshold, _millionaireThreshold - 1, () => Invalid(nameof(_richThreshold), _richThreshold));

            return;

            ExtendedException Invalid(string fieldName, int value) => new InvalidWealthMeterValueException(fieldName, value);
        }
    }
}
