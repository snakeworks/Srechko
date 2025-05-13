using UnityEngine;

public class BoardCameraTransforms : StaticInstance<BoardCameraTransforms>
{
    [SerializeField] private Transform _startingView;
    [SerializeField] private Transform _pickingOrderView;
    [SerializeField] private Transform _boardView;
    
    public static Transform StartingView => Instance._startingView;
    public static Transform PickingOrderView => Instance._pickingOrderView;
    public static Transform BoardView => Instance._boardView;
}