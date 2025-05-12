using System.Threading.Tasks;

public class StartingState : GameState
{
    public override async void OnEnter()
    {
        PlayerManager.Instance.DisableInput();
        BoardCamera.Instance.TeleportTo(BoardCameraTransforms.StartingView);
        while (SceneLoader.IsLoading)
        {
            await Task.Yield();
        }
        await Task.Delay(600);
        ChangeState(PickingPlayerOrderState);
    }

    public override void OnExit()
    {
    }
}