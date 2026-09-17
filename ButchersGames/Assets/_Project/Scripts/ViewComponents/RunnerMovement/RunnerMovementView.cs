using UnityEngine;

namespace ViewComponents.RunnerMovement
{
    public sealed class RunnerMovementView : MonoBehaviour
    {
        public void ApplyPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}
