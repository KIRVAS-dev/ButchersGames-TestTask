using R3;

namespace Core.Gameplay.WealthMeter
{
    public sealed class WealthMeterModel
    {
        public ReactiveProperty<int> Value { get; } = new ReactiveProperty<int>();
        public ReactiveProperty<WealthStage> Stage { get; } = new ReactiveProperty<WealthStage>();
    }
}
