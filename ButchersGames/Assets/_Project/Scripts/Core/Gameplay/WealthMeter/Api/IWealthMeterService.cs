namespace Core.Gameplay.WealthMeter
{
    public interface IWealthMeterService
    {
        void Increase(int amount);
        void Decrease(int amount);
    }
}
