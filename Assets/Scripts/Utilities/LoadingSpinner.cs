using DG.Tweening;
using UnityEngine;

public class LoadingSpinner : MonoBehaviour
{
    private void OnEnable()
    {
        Rotate();
        void Rotate()
        {
            transform.eulerAngles = new(0, 0, 0);
            transform.DORotate(new(0, 0, -90), 0.4f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .OnComplete(Rotate);
        }
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}
