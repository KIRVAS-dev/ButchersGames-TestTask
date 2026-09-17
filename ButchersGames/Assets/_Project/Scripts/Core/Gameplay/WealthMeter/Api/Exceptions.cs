using ExtendedExceptions;

namespace Core.Gameplay.WealthMeter
{
    public sealed class InvalidWealthMeterValueException : ExtendedException
    {
        public InvalidWealthMeterValueException(string fieldName, int value)
            : base("WealthMeter-1", $"WealthMeterConfig field '{fieldName}' has invalid value {value}") { }
    }

    public sealed class InvalidWealthMeterAmountException : ExtendedException
    {
        public InvalidWealthMeterAmountException(int amount)
            : base("WealthMeter-2", $"Amount must be positive, got {amount}") { }
    }

    public sealed class MissingWealthMeterConfigException : ExtendedException
    {
        public MissingWealthMeterConfigException(string fieldName, string objectName)
            : base("WealthMeter-3", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}
