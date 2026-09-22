using Core.Audio;
using Infrastructure.ExtendedExceptions;
using UnityEngine;
using VContainer;

namespace ViewComponents.Audio
{
    [DisallowMultipleComponent]
    public sealed class StudioListenerCameraFollower : MonoBehaviour
    {
        private IStudioListenerAnchor _anchor;

        [Inject]
        private void Construct(IStudioListenerAnchor anchor)
        {
            _anchor = anchor;
        }

        private void Awake()
        {
            Validate();
        }

        private void LateUpdate()
        {
            Transform listener = _anchor.Transform;
            listener.SetPositionAndRotation(transform.position, transform.rotation);
        }

        private void Validate()
        {
            Guard.AgainstNull(_anchor, () => new MissingStudioListenerAnchorException(gameObject.name));
            Guard.AgainstNull(_anchor.Transform, () => new MissingStudioListenerAnchorTransformException());
        }
    }
}
