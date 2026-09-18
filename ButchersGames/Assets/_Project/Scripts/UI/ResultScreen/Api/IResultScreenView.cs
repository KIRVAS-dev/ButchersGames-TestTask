namespace UI.ResultScreen
{
    public interface IResultScreenView
    {
        void Show();
        void Hide();
        void SetWinResult();
        void SetLoseResult();
        void SetMoneyAmount(int amount);
    }
}
