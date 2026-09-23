using System.Collections.Generic;
using ContentValidation;

namespace Core.Validation
{
    public sealed class SessionValidation
    {
        private readonly IReadOnlyList<IValidatable> _validatables;

        public SessionValidation(IReadOnlyList<IValidatable> validatables)
        {
            _validatables = validatables;
        }

        public void Validate()
        {
            foreach (IValidatable validatable in _validatables)
            {
                validatable.Validate();
            }
        }
    }
}
