public abstract class GameState
{
    public static StartingState StartingState = new();
    public static PickingPlayerOrderState PickingPlayerOrderState = new();
    public static NextTurnState NextTurnState = new();
    public static ChoosingBoardActionState ChoosingBoardActionState = new();

    public void ChangeState(GameState newState)
    {
        GameManager.Instance.ChangeState(newState);
    }

    public abstract void OnEnter();
    public abstract void OnExit();
}
