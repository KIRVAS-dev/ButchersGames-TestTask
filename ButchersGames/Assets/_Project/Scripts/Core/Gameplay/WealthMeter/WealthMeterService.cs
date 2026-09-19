using System;
using Core.Gameplay.LevelProgression;

namespace Core.Gameplay.WealthMeter
{
    public sealed class WealthMeterService : IWealthMeterService
    {
        private readonly ILevelProvider _levelProvider;
        private readonly IWealthMeterSettings _settings;
        private readonly WealthMeterModel _model;

        public event Action Depleted;
        public event Action<int> Increased;
        public event Action<int> Decreased;

        public WealthMeterService(
            ILevelProvider levelProvider,
            IWealthMeterSettings settings,
            WealthMeterModel model)
        {
            _levelProvider = levelProvider;
            _settings = settings;
            _model = model;
        }

        public int Value => _model.Value.Value;
        public WealthStage Stage => _model.Stage.Value;

        public void StartListening()
        {
            _levelProvider.LevelLoaded += OnLevelLoaded;
        }

        public void StopListening()
        {
            _levelProvider.LevelLoaded -= OnLevelLoaded;
        }

        public void Increase(int amount)
        {
            SetValue(_model.Value.Value + amount);

            Increased?.Invoke(amount);
        }

        public void Decrease(int amount)
        {
            SetValue(_model.Value.Value - amount);

            Decreased?.Invoke(amount);
        }

        private void OnLevelLoaded()
        {
            SetValue(_settings.StartValue);
        }

        private void SetValue(int value)
        {
            bool wasDepleted = _model.Value.Value <= 0;
            bool isDepleted = value <= 0;

            _model.Value.Value = value;
            _model.Stage.Value = StageFor(value);

            bool hasBecomeDepleted = isDepleted && !wasDepleted;

            if (hasBecomeDepleted)
            {
                Depleted?.Invoke();
            }
        }

        private WealthStage StageFor(int value)
        {
            return value switch
            {
                _ when value <= _settings.PoorThreshold => WealthStage.Poor,
                _ when value <= _settings.DescentThreshold => WealthStage.Descent,
                _ when value <= _settings.CasualThreshold => WealthStage.Casual,
                _ when value <= _settings.RichThreshold => WealthStage.Rich,
                _ => WealthStage.Millionaire
            };
        }
    }
}
