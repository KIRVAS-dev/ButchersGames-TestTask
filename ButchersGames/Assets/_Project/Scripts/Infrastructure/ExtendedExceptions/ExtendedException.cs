using System;

namespace ExtendedExceptions
{
    public class ExtendedException : Exception
    {
        protected ExtendedException(string id, string message)
            : base(message: $"{id}: {message}") { }
    }
}
