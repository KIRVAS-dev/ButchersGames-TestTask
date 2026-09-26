using R3;

namespace Core.Gameplay.WealthMeter
{
    public sealed class WealthMeterModel : IReadOnlyWealthMeterModel
    {
        public ReactiveProperty<int> WealthPoints { get; } = new ReactiveProperty<int>();
        public ReactiveProperty<WealthStage> Stage { get; } = new ReactiveProperty<WealthStage>();

        ReadOnlyReactiveProperty<int> IReadOnlyWealthMeterModel.WealthPoints => WealthPoints;
        ReadOnlyReactiveProperty<WealthStage> IReadOnlyWealthMeterModel.Stage => Stage;
    }
}
