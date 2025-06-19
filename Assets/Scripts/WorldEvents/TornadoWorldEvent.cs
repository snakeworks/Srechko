using System.Threading.Tasks;
using UnityEngine;

public class TornadoWorldEvent : WorldEvent
{
    public override async Task Apply()
    {
        await BoardCamera.Instance.TransitionTo(BoardCameraTransforms.FieldOverview, CameraTransition.Move);
        for (int i = 0; i < BoardManager.Instance.BoardPlayerControllerCount; i++)
        {
            var boardPlayer = BoardManager.Instance.GetBoardPlayerControllerAt(i);
            var currentBoardSpace = BoardSpace.Get(boardPlayer.StandingOnBoardSpaceId);
            var randomSpace =
                currentBoardSpace.transform.parent
                .GetChild(Random.Range(0, currentBoardSpace.transform.parent.childCount))
                .GetComponent<BoardSpace>();
            _ = boardPlayer.MoveToSpace(randomSpace);
        }
        await Task.Delay(20);
    }
}
