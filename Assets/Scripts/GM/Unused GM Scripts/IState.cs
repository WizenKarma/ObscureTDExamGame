/// <summary>
/// Functions for state machine states
/// </summary>
public interface IState
{
    void Enter();

    void Execute();

    void Exit();
}
