using DG.Tweening;

public class LobbyMenu : Menu
{
    private PlayerController _currentMainController;

    protected override void Init()
    {
        UpdatePlayerControls(PlayerManager.Instance.MainPlayerController);
        PlayerManager.Instance.EnableJoining();

        // Because we're listening for OnPlayerLeave, the Lobby scene can never
        // be run on it's own and must always be preceded by the MainMenu scene.
        PlayerManager.Instance.OnPlayerLeave += UpdatePlayerControls;
    }
    
    private void UpdatePlayerControls(PlayerController controller)
    {
        if (_currentMainController == controller && _currentMainController != null)
        {
            _currentMainController.InteractPerformed -= OnInteractPerformed;
            _currentMainController.CancelPerformed -= OnCancelPerformed;
        }
        if (PlayerManager.Instance.MainPlayerController != null)
        {
            _currentMainController = PlayerManager.Instance.MainPlayerController;
            _currentMainController.InteractPerformed += OnInteractPerformed;
            _currentMainController.CancelPerformed += OnCancelPerformed;
        }
    }

    private async void OnInteractPerformed()
    {
        if (ModalMenu.IsModalCurrent || MenuNavigator.IsBufferingMenuOperations)
        {
            return;
        }

        PlayerManager.Instance.DisableJoining();

        if (PlayerManager.Instance.HasMinimumPlayerCount)
        {
            var result = await ModalMenu.PushYesNo("Would you like to begin the game?");
            if (result == ModalMenu.Result.Yes)
            {
                ModalMenu.ForcePop();
            }
            else if (result == ModalMenu.Result.No)
            {
                ModalMenu.ForcePop();
            }
        }
        else
        {
            var result = await ModalMenu.PushOk("There is not enough players to start the game.");
            if (result == ModalMenu.Result.Ok)
            {
                ModalMenu.ForcePop();
            }
        }

        PlayerManager.Instance.EnableJoining();
    }

    private async void OnCancelPerformed()
    {
        if (ModalMenu.IsModalCurrent || MenuNavigator.IsBufferingMenuOperations)
        {
            return;
        }

        var result = await ModalMenu.PushYesNo("Are you sure you'd like to exit to the main menu?");
        if (result == ModalMenu.Result.Yes)
        {
            PlayerManager.Instance.CurrentOwner.CancelPerformed -= OnCancelPerformed;
            PlayerManager.Instance.DisableJoining();
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
        _currentMainController.InteractPerformed -= OnInteractPerformed;
        _currentMainController.CancelPerformed -= OnCancelPerformed;
        PlayerManager.Instance.OnPlayerLeave -= UpdatePlayerControls;
    }

    public override void TweenClose(Sequence sequence)
    {
    }

    public override void TweenOpen(Sequence sequence)
    {
    }
}
