namespace UI.Hud
{
    public interface IHudView
    {
        void Show();
        void Hide();
        void SetMoneyAmount(int amount);
        void SetWealthFillBar(float normalizedFill);
    }
}
