public class ChoosingBoardActionState : GameState
{
    public override void OnEnter()
    {
        PlayerManager.Instance.EnableInput();
        PlayerManager.Instance.GiveOwnershipTo(
            PlayerManager.Instance.GetPlayerController(BoardManager.Instance.CurrentPlayer.Index)
        );
    }

    public override void OnExit()
    {
    }
}