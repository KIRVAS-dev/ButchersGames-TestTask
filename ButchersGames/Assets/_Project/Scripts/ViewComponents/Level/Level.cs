using Infrastructure.ExtendedExceptions;
using UnityEngine;
using UnityEngine.Splines;
using ViewComponents.Finish;

namespace ViewComponents.Level
{
    public sealed class Level : MonoBehaviour
    {
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private FinishMarker _finish;
        [SerializeField] private SplineContainer _splineContainer;

        public Vector3 FinishPosition => _finish.transform.position;
        public SplineContainer SplineContainer => _splineContainer;

        private void Awake()
        {
            Validate();
        }

        private void Validate()
        {
            Guard.AgainstNull(_playerSpawnPoint, () => Missing(nameof(_playerSpawnPoint)));
            Guard.AgainstNull(_finish, () => Missing(nameof(_finish)));
            Guard.AgainstNull(_splineContainer, () => Missing(nameof(_splineContainer)));

            return;

            ExtendedException Missing(string fieldName) => new MissingLevelFieldException(fieldName, gameObject.name);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_playerSpawnPoint)
            {
                return;
            }

            Gizmos.color = Color.magenta;
            Matrix4x4 gizmosMatrix = Gizmos.matrix;
            Gizmos.matrix = _playerSpawnPoint.localToWorldMatrix;
            Gizmos.DrawSphere(Vector3.up * 0.5f + Vector3.forward, 0.5f);
            Gizmos.DrawCube(Vector3.up * 0.5f, Vector3.one);
            Gizmos.matrix = gizmosMatrix;
        }
#endif
    }
}
