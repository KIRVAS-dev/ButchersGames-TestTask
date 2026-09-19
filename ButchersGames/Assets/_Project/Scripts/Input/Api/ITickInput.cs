using System;

namespace Input
{
    public interface ITickInput
    {
        event Action<float> Ticked;
    }
}
