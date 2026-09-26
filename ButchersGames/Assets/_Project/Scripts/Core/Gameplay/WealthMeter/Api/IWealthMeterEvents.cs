using System;

namespace Core.Gameplay.WealthMeter
{
    public interface IWealthMeterEvents
    {
        event Action Depleted;
        event Action<int> Increased;
        event Action<int> Decreased;
    }
}
