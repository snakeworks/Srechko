using System.Threading.Tasks;

public class StartingState : GameState
{
    public override async void OnEnter()
    {
        PlayerManager.Instance.DisableInput();

        BoardCamera.Instance.TeleportTo(BoardCameraTransforms.StartingView1);
        await BoardCamera.Instance.TransitionTo(
            BoardCameraTransforms.StartingView2,
            CameraTransition.Move,
            5.0f,
            DG.Tweening.Ease.Linear
        );

        while (SceneLoader.IsLoading)
        {
            await Task.Yield();
        }

        await Task.Delay(600);
        AudioManager.Instance.Play(SoundName.GameStart);
        await BoardGUIAnimations.Instance.PlayPopupAnimation("GAME START!");
        await Task.Delay(600);

        ChangeState(PickingPlayerOrderState);
    }

    public override void OnExit()
    {
    }
}