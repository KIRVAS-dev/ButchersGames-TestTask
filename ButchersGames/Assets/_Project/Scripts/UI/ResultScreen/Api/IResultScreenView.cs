using System;

namespace UI.ResultScreen
{
    public interface IResultScreenView
    {
        event Action RetryClicked;
        event Action NextClicked;

        void Show();
        void Hide();
        void SetWinResult();
        void SetLoseResult();
        void SetLevelNumber(int levelNumber);
        void SetMoneyAmount(int amount);
    }
}
