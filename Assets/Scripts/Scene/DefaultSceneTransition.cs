using DG.Tweening;
using UnityEngine;

public class DefaultSceneTransition : SceneTransition
{
    private CanvasGroup _canvasGroup;

    protected override void Init()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0.0f;
    }
    
    public override void TweenIn(Sequence sequence)
    {
        sequence.Append(_canvasGroup.DOFade(1.0f, 0.5f));
    }

    public override void TweenOut(Sequence sequence)
    {
        sequence.Append(_canvasGroup.DOFade(0.0f, 0.5f));
    }
}
