namespace Core.Gameplay.WealthMeter
{
    public interface IWealthMeterSettings
    {
        int StartValue { get; }
        int PoorThreshold { get; }
        int DescentThreshold { get; }
        int CasualThreshold { get; }
        int RichThreshold { get; }
        int MillionaireThreshold { get; }
    }
}
