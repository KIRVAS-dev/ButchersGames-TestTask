using System;

namespace Core.Gameplay.WealthMeter
{
    public sealed class WealthMeterService : IWealthMeterService
    {
        private readonly WealthMeterConfig _config;
        private readonly WealthMeterModel _model;

        public event Action Depleted;

        public WealthMeterService(WealthMeterConfig config, WealthMeterModel model)
        {
            _config = config;
            _model = model;

            Reset();
        }

        public int Value => _model.Value.Value;
        public WealthStage Stage => _model.Stage.Value;

        public void Increase(int amount)
        {
            SetValue(_model.Value.Value + amount);
        }

        public void Decrease(int amount)
        {
            SetValue(_model.Value.Value - amount);
        }

        public void Reset()
        {
            SetValue(_config.StartValue);
        }

        private void SetValue(int value)
        {
            bool wasDepleted = _model.Value.Value <= 0;
            bool isDepleted = value <= 0;

            _model.Value.Value = value;
            _model.Stage.Value = StageFor(value);

            if (isDepleted && !wasDepleted)
            {
                Depleted?.Invoke();
            }
        }

        private WealthStage StageFor(int value)
        {
            return value switch
            {
                _ when value >= _config.MillionaireThreshold => WealthStage.Millionaire,
                _ when value >= _config.RichThreshold => WealthStage.Rich,
                _ when value >= _config.CasualThreshold => WealthStage.Casual,
                _ when value >= _config.DescentThreshold => WealthStage.Descent,
                _ => WealthStage.Poor
            };
        }
    }
}
