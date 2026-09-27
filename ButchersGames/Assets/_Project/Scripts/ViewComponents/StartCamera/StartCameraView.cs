using ContentValidation;
using Infrastructure.ExtendedExceptions;
using Unity.Cinemachine;
using UnityEngine;

namespace ViewComponents.StartCamera
{
    [DisallowMultipleComponent]
    public sealed class StartCameraView
        : MonoBehaviour,
          IStartCameraView,
          IValidatable
    {
        [SerializeField] private CinemachineCamera _camera;

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_camera, () => new MissingStartCameraFieldException(nameof(_camera), gameObject.name));
        }

        void IStartCameraView.ShowStart() => _camera.enabled = true;

        void IStartCameraView.ShowGameplay() => _camera.enabled = false;
    }
}
