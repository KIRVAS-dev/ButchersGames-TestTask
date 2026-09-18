using System;

namespace Core.Gameplay.WealthMeter
{
    public interface IWealthMeterService
    {
        WealthStage Stage { get; }
        int Value { get; }
        event Action Depleted;
        event Action<int> Increased;
        event Action<int> Decreased;

        void Increase(int amount);
        void Decrease(int amount);
        void Reset();
    }
}
