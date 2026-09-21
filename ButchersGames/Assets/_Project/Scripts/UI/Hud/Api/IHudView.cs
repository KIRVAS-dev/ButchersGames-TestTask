using Core.Gameplay.WealthMeter;

namespace UI.Hud
{
    public interface IHudView
    {
        void Show();
        void Hide();
        void SetLevelNumber(int levelNumber);
        void SetMoneyAmount(int amount);
        void SetWealthFillBar(float normalizedFill);
        void SetRunProgressFillBar(float normalizedFill);
        void SetWealthStage(WealthStage stage);
    }
}
