using Infrastructure.ExtendedExceptions;

namespace Core.Bootstrap.Scene
{
    internal sealed class UnhandledLoadSceneModeException : ExtendedException
    {
        public UnhandledLoadSceneModeException(LoadSceneMode loadSceneMode)
            : base("bootstrap-scene-1", $"Unhandled load scene mode: {loadSceneMode}") { }
    }

    internal sealed class SceneNotFoundException : ExtendedException
    {
        public SceneNotFoundException(string sceneName)
            : base("bootstrap-scene-2", $"Scene '{sceneName}' cannot be loaded, check Build Settings") { }
    }
}
