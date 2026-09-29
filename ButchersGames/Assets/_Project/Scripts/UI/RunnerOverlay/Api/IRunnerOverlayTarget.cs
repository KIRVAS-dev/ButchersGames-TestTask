using UnityEngine;

namespace UI.RunnerOverlay
{
    public interface IRunnerOverlayTarget
    {
        Vector2 ContainerPointOf(Vector3 worldOffset, RectTransform container);
    }
}
