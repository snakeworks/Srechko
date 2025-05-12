using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class BoardCamera : StaticInstance<BoardCamera>
{
    public bool IsTransitioning { get; private set; } = false;

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

    public void TransitionTo(Transform newTransform, CameraTransition transition = CameraTransition.Instant, float transitionDuration = 0.5f)
    {
        _camera.transform.SetParent(newTransform);
        switch (transition)
        {
            case CameraTransition.Instant:
                TeleportTo(newTransform);
                break;
            case CameraTransition.Move:
                IsTransitioning = true;
                _camera.transform.DOLocalMove(Vector3.zero, transitionDuration);
                _camera.transform.DOLocalRotate(Vector3.zero, transitionDuration).OnComplete(() => IsTransitioning = false);
                break;
        }
    }

    public void TransitionToPlayer(int index)
    {
        var player = BoardManager.Instance.GetBoardPlayerControllerAt(index);
        if (player == null)
        {
            return;
        }
        TransitionTo(player.CameraView, CameraTransition.Move);
    }
}

public enum CameraTransition
{
    Instant,
    Move   
}