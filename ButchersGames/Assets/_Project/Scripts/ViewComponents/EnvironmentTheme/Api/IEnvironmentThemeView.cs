namespace ViewComponents.EnvironmentTheme
{
    public interface IEnvironmentThemeView
    {
        int ThemeCount { get; }

        void SetTheme(int index);
    }
}
