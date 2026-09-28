using Core.Gameplay.LevelProgression;
using Core.Lifecycle;
using ViewComponents.Common;

namespace ViewComponents.EnvironmentTheme
{
    public sealed class EnvironmentThemePresenter : ISubscriptionLifecycle
    {
        private readonly ILevelLoaderEvents _levelLoaderEvents;
        private readonly IEnvironmentThemeView _view;
        private readonly NonRepeatingRandomIndex _randomIndex = new NonRepeatingRandomIndex();

        public EnvironmentThemePresenter(ILevelLoaderEvents levelLoaderEvents, IEnvironmentThemeView view)
        {
            _levelLoaderEvents = levelLoaderEvents;
            _view = view;
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
            _view.SetTheme(_randomIndex.NextIndex(_view.ThemeCount));
        }
    }
}
