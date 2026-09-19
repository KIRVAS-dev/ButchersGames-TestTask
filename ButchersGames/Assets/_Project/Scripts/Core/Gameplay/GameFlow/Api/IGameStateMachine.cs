namespace Core.Gameplay.GameFlow
{
    public interface IGameStateMachine
    {
        GameState State { get; }

        void EnterState(GameState state);
    }
}
