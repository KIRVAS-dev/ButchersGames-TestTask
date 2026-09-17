using ExtendedExceptions;

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
}
