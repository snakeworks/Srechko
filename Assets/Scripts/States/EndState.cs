using System.Threading.Tasks;

public class EndState : GameState
{
    public override async void OnEnter()
    {
        PlayerManager.Instance.DisableInput();
        await BoardCamera.Instance.TransitionTo(BoardCameraTransforms.FieldOverview);
        await BoardGUIAnimations.Instance.PlayPopupAnimation("GAME FINISHED!");
        await Task.Delay(800);
        SceneLoader.Load(Scene.Results);
    }

    public override void OnExit()
    {
    }
}