using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

[RequireComponent(typeof(Camera))]
public class BoardCamera : StaticInstance<BoardCamera>
{
    private Camera _camera;

    protected override void Awake()
    {
        base.Awake();
        _camera = GetComponent<Camera>();
    }

    public void TeleportTo(Transform newTransform)
    {
        _camera.transform.SetParent(newTransform);
        _camera.transform.localPosition = Vector3.zero;
        _camera.transform.localEulerAngles = Vector3.zero;
    }

    public async Task TransitionTo(Transform newTransform, CameraTransition transition = CameraTransition.Instant, float transitionDuration = 0.5f)
    {
        _camera.transform.SetParent(newTransform);
        switch (transition)
        {
            case CameraTransition.Instant:
                TeleportTo(newTransform);
                break;
            case CameraTransition.Move:
                Sequence sequence = DOTween.Sequence();
                sequence.Insert(0.0f, _camera.transform.DOLocalMove(Vector3.zero, transitionDuration));
                sequence.Insert(0.0f, _camera.transform.DOLocalRotate(Vector3.zero, transitionDuration));
                await sequence.AsyncWaitForCompletion();
                break;
        }
    }

    public async Task TransitionToPlayer(int index)
    {
        var player = BoardManager.Instance.GetBoardPlayerControllerAt(index);
        if (player == null)
        {
            return;
        }
        await TransitionTo(player.CameraView, CameraTransition.Move);
    }
}

public enum CameraTransition
{
    Instant,
    Move   
}