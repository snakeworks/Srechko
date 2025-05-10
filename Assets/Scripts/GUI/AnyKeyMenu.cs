using DG.Tweening;
using UnityEngine;

public class AnyKeyMenu : Menu
{
    [SerializeField] private Menu _mainMenu;

    protected override void Init()
    {
        PlayerManager.Instance.OnNewMainPlayerController += OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerController controller)
    {
        PlayerManager.Instance.OnNewMainPlayerController -= OnPlayerJoined;
        PlayerManager.Instance.DisableJoining();
        CanPop = true;
        PlayerManager.Instance.GiveOwnershipTo(controller);
        MenuNavigator.PushReplacement(_mainMenu);
    }

    public override void TweenOpen(Sequence sequence)
    {

    }

    public override void TweenClose(Sequence sequence)
    {

    }
}
