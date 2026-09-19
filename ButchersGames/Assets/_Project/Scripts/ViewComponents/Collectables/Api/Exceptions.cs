using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Collectables
{
    public sealed class MissingCollectableModifierColliderException : ExtendedException
    {
        public MissingCollectableModifierColliderException(string objectName)
            : base("collectable-1", $"Missing WealthPointsModifierCollider on {objectName}") { }
    }
}
