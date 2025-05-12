using System.Threading.Tasks;

public class NextTurnState : GameState
{
    public override async void OnEnter()
    {
        if (BoardManager.Instance.CurrentPlayerTurnIndex + 1 > PlayerManager.Instance.ControllerCount)
        {
            // ???
        }
        BoardManager.Instance.NextTurn();
        await Task.Delay(500);
        ChangeState(ChoosingBoardActionState);
    }

    public override void OnExit()
    {
    }
}
