using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MovingBoardPlayerState : GameState
{
    public override async void OnEnter()
    {
        int movesLeft = CurrentBoardPlayerController.LastRolledDiceNumber;
        PlayerManager.Instance.DisableInput();

        if (CurrentBoardPlayerController.StandingOnBoardSpaceId == -1)
        {
            movesLeft--;
            CurrentBoardPlayerController.SetFinalDiceNumberText(movesLeft);
            await CurrentBoardPlayerController.MoveToSpace(BoardManager.Instance.StartingSpace);
        }
    
        Next();

        async void Next()
        {
            movesLeft--;
            var currentSpace = BoardSpace.Get(CurrentBoardPlayerController.StandingOnBoardSpaceId);

            if (movesLeft < 0)
            {
                CurrentBoardPlayerController.HideFinalDiceNumber();
                await Task.Delay(500);
                await currentSpace.OnPlayerPassed();
                await currentSpace.OnPlayerLanded();
                ChangeState(NextTurnState);
                return;
            }

            var nextSpaces = currentSpace.GetNextSpaces();
            await currentSpace.OnPlayerPassed();
            if (nextSpaces.Count > 1)
            {
                BoardCameraTransforms.BoardView.transform.position = new(
                    CurrentBoardPlayerController.transform.position.x,
                    BoardCameraTransforms.BoardView.transform.position.y,
                    CurrentBoardPlayerController.transform.position.z - 2.0f
                );
                await BoardCamera.Instance.TransitionTo(BoardCameraTransforms.BoardView, CameraTransition.Move);

                CurrentBoardPlayerController.ShowDirectionalPrompts(nextSpaces.Keys.ToArray());

                PlayerManager.Instance.GiveOwnershipTo(CurrentController);
                PlayerManager.Instance.EnableInput();
                CurrentController.MovePerformed += OnMove;

                async void OnMove(Vector2 dir)
                {
                    BoardSpace.Direction direction = nextSpaces.First().Key;

                    // Set a threshold to avoid accidental small movements
                    const float threshold = 0.5f;
                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    {
                        if (dir.x > threshold)
                            direction = BoardSpace.Direction.Right;
                        else if (dir.x < -threshold)
                            direction = BoardSpace.Direction.Left;
                        else
                            return; // Ignore small input
                    }
                    else
                    {
                        if (dir.y > threshold)
                            direction = BoardSpace.Direction.Up;
                        else if (dir.y < -threshold)
                            direction = BoardSpace.Direction.Down;
                        else
                            return; // Ignore small input
                    }

                    if (nextSpaces.TryGetValue(direction, out var space))
                    {
                        CurrentController.MovePerformed -= OnMove;
                        PlayerManager.Instance.DisableInput();

                        await CurrentBoardPlayerController.HideDirectionalPrompts(direction);
                        await Task.Delay(200);
                        await BoardCamera.Instance.TransitionToPlayer(CurrentController.Index);

                        CurrentBoardPlayerController.SetFinalDiceNumberText(movesLeft);
                        await CurrentBoardPlayerController.MoveToSpace(space);

                        Next();
                    }
                }
            }
            else
            {
                CurrentBoardPlayerController.SetFinalDiceNumberText(movesLeft);
                await CurrentBoardPlayerController.MoveToSpace(currentSpace.GetNextSpaces().First().Value);
                Next();
            }
        }
    }

    public override void OnExit()
    {
    }
}