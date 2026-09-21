using System;
using Core.Gameplay.LevelProgression;

namespace Core.Gameplay.WealthMeter
{
    public sealed class WealthMeterService : IWealthMeterService
    {
        private readonly ILevelLoader _levelLoader;
        private readonly IWealthMeterSettings _settings;
        private readonly WealthMeterModel _model;

        public event Action Depleted;
        public event Action<int> Increased;
        public event Action<int> Decreased;

        public WealthMeterService(
            ILevelLoader levelLoader,
            IWealthMeterSettings settings,
            WealthMeterModel model)
        {
            _levelLoader = levelLoader;
            _settings = settings;
            _model = model;
        }

        public int Value => _model.WealthPoints.Value;
        public WealthStage Stage => _model.Stage.Value;

        public void StartListening()
        {
            _levelLoader.LevelLoaded += OnLevelLoaded;
        }

        public void StopListening()
        {
            _levelLoader.LevelLoaded -= OnLevelLoaded;
        }

        public void Increase(int amount)
        {
            bool hasBecomeDepleted = SetValue(_model.WealthPoints.Value + amount);

            Increased?.Invoke(amount);

            NotifyIfDepleted(hasBecomeDepleted);
        }

        public void Decrease(int amount)
        {
            bool hasBecomeDepleted = SetValue(_model.WealthPoints.Value - amount);

            Decreased?.Invoke(amount);

            NotifyIfDepleted(hasBecomeDepleted);
        }

        private void OnLevelLoaded()
        {
            NotifyIfDepleted(SetValue(_settings.StartValue));
        }

        private void NotifyIfDepleted(bool hasBecomeDepleted)
        {
            if (hasBecomeDepleted)
            {
                Depleted?.Invoke();
            }
        }

        private bool SetValue(int value)
        {
            bool wasDepleted = _model.WealthPoints.Value <= 0;
            bool isDepleted = value <= 0;

            _model.WealthPoints.Value = value;
            _model.Stage.Value = StageFor(value);

            return isDepleted && !wasDepleted;
        }

        private WealthStage StageFor(int value)
        {
            return value switch
            {
                _ when value <= _settings.PoorThreshold => WealthStage.Poor,
                _ when value <= _settings.CasualThreshold => WealthStage.Casual,
                _ when value <= _settings.MiddleThreshold => WealthStage.Middle,
                _ when value <= _settings.BusinessThreshold => WealthStage.Business,
                _ => WealthStage.Rich
            };
        }
    }
}
