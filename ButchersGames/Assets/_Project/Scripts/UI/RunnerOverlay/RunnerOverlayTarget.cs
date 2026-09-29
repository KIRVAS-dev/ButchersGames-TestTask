using UnityEngine;

namespace UI.RunnerOverlay
{
    public sealed class RunnerOverlayTarget : IRunnerOverlayTarget
    {
        private readonly Camera _worldCamera;
        private readonly Canvas _canvas;
        private readonly Transform _runner;

        public RunnerOverlayTarget(
            Camera worldCamera,
            Canvas canvas,
            Transform runner)
        {
            _worldCamera = worldCamera;
            _canvas = canvas;
            _runner = runner;
        }

        Vector2 IRunnerOverlayTarget.ContainerPointOf(Vector3 worldOffset, RectTransform container)
        {
            return CanvasPointHelper.WorldToContainerPoint(_runner.position + worldOffset, _worldCamera, _canvas, container);
        }
    }
}
