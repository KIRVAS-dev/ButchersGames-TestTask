using Core.Gameplay.Track;
using ViewComponents.Level;

namespace ViewComponents.Track
{
    public sealed class TrackProvider : ITrackProvider
    {
        private readonly LevelProvider _levelProvider;

        public TrackProvider(LevelProvider levelProvider)
        {
            _levelProvider = levelProvider;
        }

        public float Length => _levelProvider.TrackLength;

        public TrackPoint PointAt(float distance)
        {
            return _levelProvider.TrackPointAt(distance);
        }
    }
}
