using Core.Gameplay.LevelProgression;
using Core.Lifecycle;
using ViewComponents.Level;

namespace ViewComponents.TrackColor
{
    public sealed class TrackColorPresenter : ISubscriptionLifecycle
    {
        private readonly ILevelLoaderEvents _levelLoaderEvents;
        private readonly CurrentLevel _currentLevel;
        private readonly TrackColorPicker _picker = new TrackColorPicker();

        public TrackColorPresenter(ILevelLoaderEvents levelLoaderEvents, CurrentLevel currentLevel)
        {
            _levelLoaderEvents = levelLoaderEvents;
            _currentLevel = currentLevel;
        }

        void ISubscriptionLifecycle.Start()
        {
            _levelLoaderEvents.LevelLoaded += OnLevelLoaded;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _levelLoaderEvents.LevelLoaded -= OnLevelLoaded;
        }

        private void OnLevelLoaded()
        {
            ITrackColorView view = _currentLevel.TrackColorView;

            view.SetColor(_picker.NextIndex(view.ColorCount));
        }
    }
}
