using Core.Gameplay.GameFlow;
using Infrastructure.ExtendedExceptions;

namespace ViewComponents.CelebrationCamera
{
    internal sealed class InvalidCelebrationCameraValueException : ExtendedException
    {
        internal InvalidCelebrationCameraValueException(string fieldName, float value)
            : base("celebration-camera-1", $"CelebrationCameraConfig field '{fieldName}' has invalid value {value}") { }
    }

    internal sealed class MissingCelebrationCameraConfigException : ExtendedException
    {
        internal MissingCelebrationCameraConfigException(string fieldName, string objectName)
            : base("celebration-camera-2", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class MissingCelebrationCameraFollowException : ExtendedException
    {
        internal MissingCelebrationCameraFollowException(string fieldName, string objectName)
            : base("celebration-camera-3", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class MissingCelebrationCameraTargetException : ExtendedException
    {
        internal MissingCelebrationCameraTargetException(string fieldName, string objectName)
            : base("celebration-camera-7", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class InvalidCelebrationCameraRadiusException : ExtendedException
    {
        internal InvalidCelebrationCameraRadiusException(string objectName, float radius)
            : base("celebration-camera-4", $"Horizontal distance '{radius}' to the pivot must be positive on '{objectName}'") { }
    }

    internal sealed class CelebrationCameraArcTooLongException : ExtendedException
    {
        internal CelebrationCameraArcTooLongException(float arcDistance, float radius)
            : base(
                "celebration-camera-5",
                $"Arc distance '{arcDistance}' must be shorter than half an orbit of radius '{radius}'"
            ) { }
    }

    internal sealed class UnhandledCelebrationCameraStateException : ExtendedException
    {
        internal UnhandledCelebrationCameraStateException(GameState state)
            : base("celebration-camera-6", $"GameState '{state}' is not handled by the celebration camera presenter") { }
    }
}
