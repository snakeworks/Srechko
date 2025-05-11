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

    private void OnCancelPerformed()
    {
        PlayerManager.Instance.CurrentOwner.CancelPerformed -= OnCancelPerformed;
        SceneLoader.Load(Scene.MainMenu, SceneTransition.Get(), false);
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
