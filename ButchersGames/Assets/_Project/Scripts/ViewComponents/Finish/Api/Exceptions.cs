using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Finish
{
    public sealed class MissingFinishColliderException : ExtendedException
    {
        public MissingFinishColliderException(string objectName)
            : base("finish-1", $"No active FinishCollider found in loaded level for {objectName}") { }
    }
}
