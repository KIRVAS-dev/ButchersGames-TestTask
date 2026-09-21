using UnityEngine;

namespace ViewComponents.Track
{
    public readonly struct TrackPoint
    {
        public TrackPoint(Vector3 position, Vector3 forward)
        {
            Position = position;
            Forward = forward;
        }

        public Vector3 Position { get; }
        public Vector3 Forward { get; }
    }
}
