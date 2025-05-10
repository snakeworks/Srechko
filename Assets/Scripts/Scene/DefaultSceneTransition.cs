using DG.Tweening;

public class DefaultSceneTransition : SceneTransition
{
    protected override void Init()
    {
        transform.DOLocalMoveX(-1920.0f, 0.0f);
    }
    
    public override void TweenIn(Sequence sequence)
    {
        sequence.Append(transform.DOLocalMoveX(0.0f, 0.25f));
    }

    public override void TweenOut(Sequence sequence)
    {
        sequence.Append(transform.DOLocalMoveX(1920.0f, 0.25f));
        sequence.Append(transform.DOLocalMoveX(-1920.0f, 0.0f));
    }
}
