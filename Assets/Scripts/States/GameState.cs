public abstract class GameState
{
    public static StartingState StartingState = new();
    public static PickingPlayerOrderState PickingPlayerOrderState = new();
    public static NextTurnState NextTurnState = new();
    public static ChoosingBoardActionState ChoosingBoardActionState = new();
    public static RollingDiceState RollingDiceState = new();
    public static MovingBoardPlayerState MovingBoardPlayerState = new();

    protected PlayerController CurrentController => PlayerManager.Instance.GetPlayerController(BoardManager.Instance.CurrentPlayer.Index);
    protected BoardPlayerData CurrentPlayerData => GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index);
    protected BoardPlayerController CurrentBoardPlayerController => BoardManager.Instance.CurrentPlayer;

    public void ChangeState(GameState newState)
    {
        GameManager.Instance.ChangeState(newState);
    }

    public abstract void OnEnter();
    public abstract void OnExit();
}
