using Core.Gameplay.WealthMeter;

namespace UI.WealthIndicator
{
    public interface IWealthIndicatorView
    {
        void Show();
        void Hide();
        void SetFillBar(float normalizedFill);
        void SetStage(WealthStage stage);
    }
}
