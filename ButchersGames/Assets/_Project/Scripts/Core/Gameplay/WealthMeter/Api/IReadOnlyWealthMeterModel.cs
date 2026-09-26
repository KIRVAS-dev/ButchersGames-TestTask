using R3;

namespace Core.Gameplay.WealthMeter
{
    public interface IReadOnlyWealthMeterModel
    {
        ReadOnlyReactiveProperty<int> WealthPoints { get; }
        ReadOnlyReactiveProperty<WealthStage> Stage { get; }
    }
}
