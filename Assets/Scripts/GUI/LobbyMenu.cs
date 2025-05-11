using DG.Tweening;

public class LobbyMenu : Menu
{
    private PlayerController _currentMainController;

    protected override void Init()
    {
        OnPlayerLeave(PlayerManager.Instance.MainPlayerController);
        PlayerManager.Instance.EnableJoining();
        PlayerManager.Instance.OnPlayerLeave += OnPlayerLeave;
    }
    
    private void OnPlayerLeave(PlayerController controller)
    {
        if (_currentMainController == controller)
        {
            _currentMainController.CancelPerformed -= OnCancelPerformed;
        }
        if (PlayerManager.Instance.MainPlayerController != null)
        {
            _currentMainController = PlayerManager.Instance.MainPlayerController;
            _currentMainController.CancelPerformed += OnCancelPerformed;
        }
    }

    private async void OnCancelPerformed()
    {
        if (ModalMenu.IsModalCurrent)
        {
            return;
        }

        var result = await ModalMenu.PushYesNo("Are you sure you'd like to exit to the main menu?");
        if (result == ModalMenu.Result.Yes)
        {
            PlayerManager.Instance.CurrentOwner.CancelPerformed -= OnCancelPerformed;
            SceneLoader.Load(Scene.MainMenu, SceneTransition.Get(), false);
        }
        else
        {
            ModalMenu.ForcePop();
        }
    }

    private void OnDestroy()
    {
        if (PlayerManager.Instance == null)
        {
            return;
        }
        _currentMainController.CancelPerformed -= OnCancelPerformed;
        PlayerManager.Instance.OnPlayerLeave -= OnPlayerLeave;
    }

    public override void TweenClose(Sequence sequence)
    {
        PlayerManager.Instance.DisableJoining();
    }

    public override void TweenOpen(Sequence sequence)
    {
    }
}
