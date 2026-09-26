using System;

namespace Core.Gameplay.RunnerMovement
{
    public interface IRunnerMovementEvents
    {
        event Action PositionReset;
    }
}
