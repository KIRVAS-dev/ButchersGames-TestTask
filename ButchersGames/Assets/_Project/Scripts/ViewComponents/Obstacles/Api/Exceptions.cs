using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Obstacles
{
    public sealed class MissingObstacleConfigException : ExtendedException
    {
        public MissingObstacleConfigException(string fieldName, string objectName)
            : base("obstacle-1", $"Missing field {fieldName} on {objectName}") { }
    }

    public sealed class InvalidObstacleValueException : ExtendedException
    {
        public InvalidObstacleValueException(string fieldName, float value)
            : base("obstacle-2", $"ObstacleConfig field '{fieldName}' has invalid value {value}") { }
    }

    public sealed class MissingObstacleModifierColliderException : ExtendedException
    {
        public MissingObstacleModifierColliderException(string objectName)
            : base("obstacle-3", $"Missing WealthPointsModifierCollider on {objectName}") { }
    }
}
