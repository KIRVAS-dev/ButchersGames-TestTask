using System;

namespace Input
{
    public interface IDragInput
    {
        event Action<float> DragNormalizedOffsetChanged;
    }
}
