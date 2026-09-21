using Infrastructure.ExtendedExceptions;

namespace Core.Bootstrap.Scene
{
    public sealed class UnhandledLoadSceneModeException : ExtendedException
    {
        public UnhandledLoadSceneModeException(LoadSceneMode loadSceneMode)
            : base("bootstrap-scene-1", $"Unhandled load scene mode: {loadSceneMode}") { }
    }

    public sealed class SceneNotFoundException : ExtendedException
    {
        public SceneNotFoundException(string sceneName)
            : base("bootstrap-scene-2", $"Scene '{sceneName}' cannot be loaded, check Build Settings") { }
    }
}
