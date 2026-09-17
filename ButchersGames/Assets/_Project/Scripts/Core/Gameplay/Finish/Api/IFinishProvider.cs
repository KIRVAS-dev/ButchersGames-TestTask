using System;

namespace Core.Gameplay.Finish
{
    public interface IFinishProvider
    {
        event Action Reached;
    }
}
