using DG.Tweening;

public class LobbyMenu : Menu
{
    protected override void Init()
    {
        PlayerManager.Instance.CurrentOwner.CancelPerformed += OnCancelPerformed;
    }

    private void OnCancelPerformed()
    {
        PlayerManager.Instance.CurrentOwner.CancelPerformed -= OnCancelPerformed;
        SceneLoader.Load(Scene.MainMenu);
    }

    public override void TweenClose(Sequence sequence)
    {
    }

    public override void TweenOpen(Sequence sequence)
    {
    }
}
