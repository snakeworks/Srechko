using System.Linq;

public class MovingBoardPlayerState : GameState
{
    public override void OnEnter()
    {
        int movesLeft = CurrentBoardPlayerController.LastRolledDiceNumber;
        PlayerManager.Instance.DisableInput();

        if (CurrentBoardPlayerController.StandingOnBoardSpaceId == -1)
        {
            movesLeft--;
            CurrentBoardPlayerController.MoveToSpace(BoardManager.Instance.StartingSpace, Next);
        }
        else
        {
            Next();
        }

        void Next()
        {
            movesLeft--;

            if (movesLeft < 0)
            {
                ChangeState(NextTurnState);
                return;
            }

            var currentSpace = BoardSpace.Get(CurrentBoardPlayerController.StandingOnBoardSpaceId);
            CurrentBoardPlayerController.MoveToSpace(currentSpace.GetNextSpaces().First().Key, Next);
        }
    }

    public override void OnExit()
    {
    }
}