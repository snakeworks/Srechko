using System.Threading.Tasks;

public class ChoosingBoardActionState : GameState
{
    public override async void OnEnter()
    {
        BoardCamera.Instance.TransitionToPlayer(CurrentBoardPlayerController.Index);
        BoardManager.Instance.BoardActionMenu.ResetLastSelectedObject();
        BoardManager.Instance.BoardActionMenu.ItemMenu.ResetLastSelectedObject();

        while (BoardCamera.Instance.IsTransitioning)
        {
            await Task.Yield();
        }
     
        await BoardGUIAnimations.Instance.AnimatePlayerTurn();
        
        await Task.Delay(250);

        PlayerManager.Instance.EnableInput();
        PlayerManager.Instance.GiveOwnershipTo(CurrentController);

        BoardManager.Instance.BoardActionMenu.OnDiceRollPressed += OnDiceRollPressed;
        BoardManager.Instance.BoardActionMenu.ItemMenu.OnItemPressed += OnItemPressed;
        BoardManager.Instance.BoardActionMenu.Push();
    }

    private void ResetListeners()
    {
        PlayerManager.Instance.DisableInput();
        BoardManager.Instance.BoardActionMenu.OnDiceRollPressed -= OnDiceRollPressed;
        BoardManager.Instance.BoardActionMenu.ItemMenu.OnItemPressed -= OnItemPressed;
    }

    private async void OnDiceRollPressed()
    {
        ResetListeners();
        BoardManager.Instance.BoardActionMenu.ForcePop();
        await Task.Delay(100);
        ChangeState(RollingDiceState);
    }

    private async void OnItemPressed()
    {
        ResetListeners();
        MenuNavigator.ForcePopUntilEmpty();
        await Task.Delay(100);
        ChangeState(UseItemState);
    }

    public override void OnExit()
    {
    }
}