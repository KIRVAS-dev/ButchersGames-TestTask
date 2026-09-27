using Infrastructure.ExtendedExceptions;

namespace Core.Loading
{
    internal sealed class InvalidLoadingCompletionException : ExtendedException
    {
        internal InvalidLoadingCompletionException()
            : base("loading-1", "Loading is already completed") { }
    }
}
