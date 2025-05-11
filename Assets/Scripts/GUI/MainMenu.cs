using DG.Tweening;
using UnityEngine;

public class MainMenu : Menu
{
    protected override void Init()
    {
        if (PlayerManager.Instance.MainPlayerController != null)
        {
            OpenImmediate = true;
            PlayerManager.Instance.GiveOwnershipTo(PlayerManager.Instance.MainPlayerController);
        }
    }

    public override void TweenOpen(Sequence sequence)
    {
    }

    public override void TweenClose(Sequence sequence)
    {
    }

    public void PlayPressed()
    {
        SceneLoader.Load(Scene.Lobby, SceneTransition.Get(), false);
    }

    public void QuitPressed()
    {
        Application.Quit();
    }
}
