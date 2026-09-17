using UnityEngine;
using UnityEngine.Serialization;

namespace ViewComponents.Level
{
    public sealed class Level : MonoBehaviour
    {
        [FormerlySerializedAs("playerSpawnPoint")] [SerializeField] private Transform _playerSpawnPoint;

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
