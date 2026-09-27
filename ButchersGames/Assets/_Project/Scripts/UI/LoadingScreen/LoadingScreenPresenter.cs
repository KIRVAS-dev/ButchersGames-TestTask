using Core.Gameplay.LevelProgression;
using Core.Lifecycle;

namespace UI.LoadingScreen
{
    public sealed class LoadingScreenPresenter : ISubscriptionLifecycle
    {
        private readonly ILoadingScreenView _view;
        private readonly ILevelLoaderEvents _levelLoaderEvents;

        public LoadingScreenPresenter(ILoadingScreenView view, ILevelLoaderEvents levelLoaderEvents)
        {
            _view = view;
            _levelLoaderEvents = levelLoaderEvents;
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
            _levelLoaderEvents.LevelLoaded -= OnLevelLoaded;
            _view.Hide();
        }
    }
}
