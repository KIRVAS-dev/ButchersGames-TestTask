using Infrastructure.ExtendedExceptions;

namespace ViewComponents.EnvironmentTheme
{
    internal sealed class EmptyEnvironmentThemeConfigException : ExtendedException
    {
        internal EmptyEnvironmentThemeConfigException(string configName)
            : base("environment-theme-1", $"Environment theme config {configName} is empty") { }
    }

    internal sealed class MissingEnvironmentThemeTextureException : ExtendedException
    {
        internal MissingEnvironmentThemeTextureException(
            string configName,
            int index,
            string textureName)
            : base("environment-theme-2", $"Theme {index} in {configName} has no {textureName} texture") { }
    }

    internal sealed class MissingEnvironmentThemeFieldException : ExtendedException
    {
        internal MissingEnvironmentThemeFieldException(string fieldName, string objectName)
            : base("environment-theme-3", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}
