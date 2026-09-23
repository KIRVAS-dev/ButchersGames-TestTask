using ContentValidation;
using Infrastructure.ExtendedExceptions;
using UnityEngine;
using UnityEngine.Splines;
using ViewComponents.Track;

namespace ViewComponents.Level
{
    public sealed class Level
        : MonoBehaviour,
          IValidatable
    {
        [SerializeField] private StartMarker _start;
        [SerializeField] private FinishMarker _finish;
        [SerializeField] private SplineContainer _splineContainer;

        public Vector3 StartPosition => _start.transform.position;
        public Vector3 FinishPosition => _finish.transform.position;
        public SplineContainer SplineContainer => _splineContainer;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_start, () => Missing(nameof(_start)));
            Guard.AgainstNull(_finish, () => Missing(nameof(_finish)));
            Guard.AgainstNull(_splineContainer, () => Missing(nameof(_splineContainer)));

            return;

            ExtendedException Missing(string fieldName) => new MissingLevelFieldException(fieldName, gameObject.name);
        }
    }
}
