using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class TornadoWorldEvent : WorldEvent
{
    [SerializeField] private SpriteRenderer _tornadoSprite;

    private void Awake()
    {
        _tornadoSprite.DOFade(0.0f, 0.0f);
    }

    public override async Task Apply()
    {
        await BoardCamera.Instance.TransitionTo(BoardCameraTransforms.FieldOverview, CameraTransition.Move);
        await Task.Delay(100);
        _tornadoSprite.transform
            .DORotate(new Vector3(90.0f, 0.0f, 360), 0.5f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
        _tornadoSprite.DOFade(0.85f, 0.1f);
        AudioManager.Instance.Play(SoundName.Wind);
        for (int i = 0; i < BoardManager.Instance.BoardPlayerControllerCount; i++)
        {
            var boardPlayer = BoardManager.Instance.GetBoardPlayerControllerAt(i);
            _ = boardPlayer.MoveToSpace(BoardSpace.GetRandom());
        }
        await Task.Delay(1300);
        await _tornadoSprite.DOFade(0.0f, 0.1f).AsyncWaitForCompletion();
        _tornadoSprite.transform.DOKill();
    }
}
