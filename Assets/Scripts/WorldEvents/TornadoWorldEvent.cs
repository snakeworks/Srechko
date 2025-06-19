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
            _ = boardPlayer.MoveToSpace(BoardSpace.GetRandom());
        }
        await Task.Delay(20);
    }
}
