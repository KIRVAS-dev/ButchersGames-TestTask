namespace UI
{
    internal static class LevelNumberTextHelper
    {
        private const string LevelNumberTextFormat = "Уровень {0}";

        public static string Format(int levelNumber)
        {
            return string.Format(LevelNumberTextFormat, levelNumber);
        }
    }
}
