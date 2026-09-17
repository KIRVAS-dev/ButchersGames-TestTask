using ExtendedExceptions;

namespace Core.Gameplay.WealthMeter
{
    public sealed class InvalidWealthMeterValueException : ExtendedException
    {
        public InvalidWealthMeterValueException(string fieldName, int value)
            : base("wealth-meter-1", $"WealthMeterConfig field '{fieldName}' has invalid value {value}") { }
    }

    public sealed class MissingWealthMeterConfigException : ExtendedException
    {
        public MissingWealthMeterConfigException(string fieldName, string objectName)
            : base("wealth-meter-2", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}
