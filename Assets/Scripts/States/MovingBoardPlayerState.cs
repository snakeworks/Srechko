using System.Linq;
using System.Threading.Tasks;

public class MovingBoardPlayerState : GameState
{
    public override void OnEnter()
    {
        int movesLeft = CurrentBoardPlayerController.LastRolledDiceNumber;
        PlayerManager.Instance.DisableInput();

        if (CurrentBoardPlayerController.StandingOnBoardSpaceId == -1)
        {
            movesLeft--;
            CurrentBoardPlayerController.SetDiceNumberText(movesLeft);
            CurrentBoardPlayerController.MoveToSpace(BoardManager.Instance.StartingSpace, Next);
        }
        else
        {
            Next();
        }

        async void Next()
        {
            movesLeft--;

            if (movesLeft < 0)
            {
                CurrentBoardPlayerController.HideDice();
                await Task.Delay(100);
                ChangeState(NextTurnState);
                return;
            }

            CurrentBoardPlayerController.SetDiceNumberText(movesLeft);

            var currentSpace = BoardSpace.Get(CurrentBoardPlayerController.StandingOnBoardSpaceId);
            CurrentBoardPlayerController.MoveToSpace(currentSpace.GetNextSpaces().First().Key, Next);
        }
    }

    public override void OnExit()
    {
    }
}