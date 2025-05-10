using UnityEngine;
using DG.Tweening;

public class PauseMenu : Menu
{
    protected override void Init()
    {
        _canvasGroup.alpha = 0.0f;
    }

    public override void TweenOpen(Sequence sequence)
    {
        sequence.Append(_canvasGroup.DOFade(1.0f, 0.2f));
    }
    
    public override void TweenClose(Sequence sequence)
    {
        sequence.Append(_canvasGroup.DOFade(0.0f, 0.2f));
    }
}
