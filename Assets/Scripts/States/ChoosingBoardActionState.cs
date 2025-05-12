using System.Threading.Tasks;

public class ChoosingBoardActionState : GameState
{
    public override async void OnEnter()
    {
        PlayerManager.Instance.EnableInput();
        PlayerManager.Instance.GiveOwnershipTo(CurrentController);
        BoardCamera.Instance.TransitionToPlayer(CurrentBoardPlayerController.Index);
    
        while (BoardCamera.Instance.IsTransitioning)
        {
            await Task.Yield();
        }

        await Task.Delay(250);

        BoardManager.Instance.BoardPlayerActionMenu.OnDiceRollPressed += OnDiceRollPressed;
        BoardManager.Instance.BoardPlayerActionMenu.PushWithBoardPlayerData(CurrentPlayerData);
    }

    private async void OnDiceRollPressed()
    {
        BoardManager.Instance.BoardPlayerActionMenu.OnDiceRollPressed -= OnDiceRollPressed;
        BoardManager.Instance.BoardPlayerActionMenu.ForcePop();
        await Task.Delay(100);
        ChangeState(RollingDiceState);
    }

    public override void OnExit()
    {
    }
}