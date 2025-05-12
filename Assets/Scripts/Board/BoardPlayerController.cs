using UnityEngine;

public class BoardPlayerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _visuals;
    [SerializeField] private Transform _cameraView;

    public Transform CameraView => _cameraView;
    public int Index => transform.GetSiblingIndex();

    private void Awake()
    {
        
    }
}
