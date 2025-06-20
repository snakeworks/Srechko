using UnityEngine;

public class BoardCameraTransforms : StaticInstance<BoardCameraTransforms>
{
    [SerializeField] private Transform _startingView1;
    [SerializeField] private Transform _startingView2;
    [SerializeField] private Transform _pickingOrderView;
    [SerializeField] private Transform _boardView;
    [SerializeField] private Transform _fieldOverview;

    public static Transform StartingView1 => Instance._startingView1;
    public static Transform StartingView2 => Instance._startingView2;
    public static Transform PickingOrderView => Instance._pickingOrderView;
    public static Transform BoardView => Instance._boardView;
    public static Transform FieldOverview => Instance._fieldOverview;
}