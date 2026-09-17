using System;

namespace Core.Gameplay.WealthMeter
{
    public interface IWealthMeterService
    {
        WealthStage Stage { get; }
        int Value { get; }
        event Action Depleted;

        void Increase(int amount);
        void Decrease(int amount);
        void Reset();
    }
}
