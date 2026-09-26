using ContentValidation;
using Infrastructure.ExtendedExceptions;
using UnityEngine;
using UnityEngine.Splines;
using ViewComponents.Track;
using ViewComponents.TrackColor;

namespace ViewComponents.Level
{
    public sealed class Level
        : MonoBehaviour,
          IValidatable
    {
        [SerializeField] private StartMarker _start;
        [SerializeField] private FinishMarker _finish;
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private TrackColorView _trackColorView;

        public Vector3 StartPosition => _start.transform.position;
        public Vector3 FinishPosition => _finish.transform.position;
        public SplineContainer SplineContainer => _splineContainer;
        internal ITrackColorView TrackColorView => _trackColorView;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_start, () => Missing(nameof(_start)));
            Guard.AgainstNull(_finish, () => Missing(nameof(_finish)));
            Guard.AgainstNull(_splineContainer, () => Missing(nameof(_splineContainer)));
            Guard.AgainstNull(_trackColorView, () => Missing(nameof(_trackColorView)));

            return;

            ExtendedException Missing(string fieldName) => new MissingLevelFieldException(fieldName, gameObject.name);
        }
    }
}
