using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Level
{
    internal sealed class MissingLevelListConfigException : ExtendedException
    {
        public MissingLevelListConfigException(string fieldName, string objectName)
            : base("level-1", $"Missing field {fieldName} on {objectName}") { }
    }

    internal sealed class MissingLevelPrefabException : ExtendedException
    {
        public MissingLevelPrefabException(int index, string objectName)
            : base("level-2", $"Level prefab at index {index} is not assigned on {objectName}") { }
    }

    internal sealed class MissingLevelFieldException : ExtendedException
    {
        public MissingLevelFieldException(string fieldName, string objectName)
            : base("level-3", $"Missing field {fieldName} on {objectName}") { }
    }

    internal sealed class EmptyLevelListException : ExtendedException
    {
        public EmptyLevelListException(string configName)
            : base("level-4", $"Level list {configName} is empty") { }
    }

    internal sealed class InvalidLevelRunException : ExtendedException
    {
        public InvalidLevelRunException(
            string objectName,
            float startCoordinate,
            float finishCoordinate)
            : base(
                "level-5",
                $"Finish coordinate {finishCoordinate} must be greater than start coordinate {startCoordinate} on {objectName}"
            ) { }
    }
}
