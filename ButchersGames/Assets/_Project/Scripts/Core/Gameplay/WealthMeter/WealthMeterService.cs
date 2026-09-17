using System;

namespace Core.Gameplay.WealthMeter
{
    public sealed class WealthMeterService : IWealthMeterService
    {
        private readonly IWealthMeterSettings _settings;
        private readonly WealthMeterModel _model;

        public event Action Depleted;

        public WealthMeterService(IWealthMeterSettings settings, WealthMeterModel model)
        {
            _settings = settings;
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
            SetValue(_settings.StartValue);
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
                _ when value >= _settings.MillionaireThreshold => WealthStage.Millionaire,
                _ when value >= _settings.RichThreshold => WealthStage.Rich,
                _ when value >= _settings.CasualThreshold => WealthStage.Casual,
                _ when value >= _settings.DescentThreshold => WealthStage.Descent,
                _ => WealthStage.Poor
            };
        }
    }
}
