using Core.Gameplay.GameFlow;
using Infrastructure.ExtendedExceptions;

namespace ViewComponents.StartCamera
{
    internal sealed class MissingStartCameraFieldException : ExtendedException
    {
        internal MissingStartCameraFieldException(string fieldName, string objectName)
            : base("start-camera-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class UnhandledStartCameraStateException : ExtendedException
    {
        internal UnhandledStartCameraStateException(GameState state)
            : base("start-camera-2", $"GameState '{state}' is not handled by the start camera presenter") { }
    }
}
