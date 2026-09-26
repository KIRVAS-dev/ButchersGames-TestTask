using System;
using Core.Gameplay.LevelProgression;
using Core.Lifecycle;

namespace Core.Gameplay.WealthMeter
{
    public sealed class WealthMeterService
        : IWealthMeterService,
          IWealthMeterEvents,
          ISubscriptionLifecycle
    {
        private readonly ILevelLoaderEvents _levelLoaderEvents;
        private readonly IWealthMeterSettings _settings;
        private readonly WealthMeterModel _model;

        public event Action Depleted;
        public event Action<int> Increased;
        public event Action<int> Decreased;

        public WealthMeterService(
            ILevelLoaderEvents levelLoaderEvents,
            IWealthMeterSettings settings,
            WealthMeterModel model)
        {
            _levelLoaderEvents = levelLoaderEvents;
            _settings = settings;
            _model = model;
        }

        void ISubscriptionLifecycle.Start()
        {
            _levelLoaderEvents.LevelLoaded += OnLevelLoaded;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _levelLoaderEvents.LevelLoaded -= OnLevelLoaded;
        }

        void IWealthMeterService.Increase(int amount)
        {
            bool hasBecomeDepleted = SetValue(_model.WealthPoints.Value + amount);

            Increased?.Invoke(amount);

            NotifyIfDepleted(hasBecomeDepleted);
        }

        void IWealthMeterService.Decrease(int amount)
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
            int clampedValue = Math.Max(0, value);
            bool wasDepleted = _model.WealthPoints.Value <= 0;
            bool isDepleted = clampedValue <= 0;

            _model.WealthPoints.Value = clampedValue;
            _model.Stage.Value = StageFor(clampedValue);

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
