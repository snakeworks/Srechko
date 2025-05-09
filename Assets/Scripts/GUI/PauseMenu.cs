using System;
using UnityEngine;
using DG.Tweening;

public class PauseMenu : Menu
{
    [SerializeField] private CanvasGroup _canvasGroup;

    protected override void Init()
    {
        _canvasGroup.alpha = 0.0f;
    }

    public override void TweenOpen(Action onComplete)
    {
        _canvasGroup.DOFade(1.0f, 0.2f).OnComplete(() => onComplete());
    }
    
    public override void TweenClose(Action onComplete)
    {
        _canvasGroup.DOFade(0.0f, 0.2f).OnComplete(() => onComplete());
    }
}
