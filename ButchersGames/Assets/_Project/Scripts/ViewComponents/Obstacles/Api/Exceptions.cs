using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Obstacles
{
    internal sealed class MissingObstacleConfigException : ExtendedException
    {
        public MissingObstacleConfigException(string fieldName, string objectName)
            : base("obstacle-1", $"Missing field {fieldName} on {objectName}") { }
    }

    internal sealed class InvalidObstacleValueException : ExtendedException
    {
        public InvalidObstacleValueException(string fieldName, float value)
            : base("obstacle-2", $"ObstacleConfig field '{fieldName}' has invalid value {value}") { }
    }

    internal sealed class MissingObstacleModifierColliderException : ExtendedException
    {
        public MissingObstacleModifierColliderException(string objectName)
            : base("obstacle-3", $"Missing WealthPointsModifierCollider on {objectName}") { }
    }
}
