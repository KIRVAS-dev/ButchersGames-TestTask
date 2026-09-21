namespace Core.Gameplay.WealthMeter
{
    public interface IWealthMeterSettings
    {
        int StartValue { get; }
        int PoorThreshold { get; }
        int CasualThreshold { get; }
        int MiddleThreshold { get; }
        int BusinessThreshold { get; }
        int RichThreshold { get; }
    }
}
