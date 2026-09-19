using System;

namespace UI.StartScreen
{
    public interface IStartScreenView
    {
        event Action StartClicked;

        void Show();
        void Hide();
        void SetLevelNumber(int levelNumber);
    }
}
