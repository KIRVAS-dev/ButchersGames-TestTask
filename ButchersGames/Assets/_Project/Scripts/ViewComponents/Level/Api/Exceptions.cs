using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Level
{
    public sealed class MissingLevelListConfigException : ExtendedException
    {
        public MissingLevelListConfigException(string fieldName, string objectName)
            : base("level-1", $"Missing field {fieldName} on {objectName}") { }
    }

    public sealed class MissingLevelPrefabException : ExtendedException
    {
        public MissingLevelPrefabException(int index, string objectName)
            : base("level-2", $"Level prefab at index {index} is not assigned on {objectName}") { }
    }

    public sealed class EmptyLevelListException : ExtendedException
    {
        public EmptyLevelListException(string configName)
            : base("level-4", $"Level list {configName} is empty") { }
    }

    public sealed class LevelNotLoadedException : ExtendedException
    {
        public LevelNotLoadedException(string objectName)
            : base("level-5", $"Current level is requested before any level is loaded on {objectName}") { }
    }

    public sealed class MissingLevelProviderReferenceException : ExtendedException
    {
        public MissingLevelProviderReferenceException(string fieldName, string objectName)
            : base("level-3", $"Missing field {fieldName} on {objectName}") { }
    }
}
