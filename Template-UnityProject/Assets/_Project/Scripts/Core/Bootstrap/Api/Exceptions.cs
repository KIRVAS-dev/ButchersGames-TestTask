using ExtendedExceptions;

namespace Core.Bootstrap
{
    public sealed class UnhandledLoadSceneModeException : ExtendedException
    {
        public UnhandledLoadSceneModeException(LoadSceneMode loadSceneMode)
            : base("bootstrap-scene-1", $"Unhandled load scene mode: {loadSceneMode}") { }
    }
}
