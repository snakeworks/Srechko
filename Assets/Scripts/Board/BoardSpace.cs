using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BoardSpace : MonoBehaviour
{
    [SerializeField] private BoardSpace _nextSpaceUp;
    [SerializeField] private BoardSpace _nextSpaceDown;
    [SerializeField] private BoardSpace _nextSpaceLeft;
    [SerializeField] private BoardSpace _nextSpaceRight;

    public int Id { get; private set; }

    private readonly List<BoardPlayerController> _standingBoardPlayers = new();
    private static Transform _parent;

    private void Awake()
    {
        Id = transform.GetSiblingIndex();
        _parent = transform.parent;
    }

    public static BoardSpace Get(int id)
    {
        return _parent.GetChild(id).GetComponent<BoardSpace>();
    }

    public Dictionary<Direction, BoardSpace> GetNextSpaces()
    {
        Dictionary<Direction, BoardSpace> spaces = new();
        if (_nextSpaceUp != null) spaces.Add(Direction.Up, _nextSpaceUp);
        if (_nextSpaceDown != null) spaces.Add(Direction.Down, _nextSpaceDown);
        if (_nextSpaceLeft != null) spaces.Add(Direction.Left, _nextSpaceLeft);
        if (_nextSpaceRight != null) spaces.Add(Direction.Right, _nextSpaceRight);

        return spaces;
    }

    public void OnPlayerEntered(BoardPlayerController boardPlayer)
    {
        _standingBoardPlayers.Add(boardPlayer);
        UpdateBoardPlayerVisuals();
    }

    public void OnPlayerExited(BoardPlayerController boardPlayer)
    {
        _standingBoardPlayers.Remove(boardPlayer);
        UpdateBoardPlayerVisuals();
    }

    private void UpdateBoardPlayerVisuals()
    {
        if (_standingBoardPlayers.Count == 1)
        {
            _standingBoardPlayers[0].UpdateVisualsOffset(Vector3.zero);
            return;
        }
        for (int i = 0; i < _standingBoardPlayers.Count; i++)
        {
            float angle = i * Mathf.PI * 2 / _standingBoardPlayers.Count + Mathf.PI / 4; // Add a diagonal offset
            float radius = 0.2f;
            float offsetX = Mathf.Cos(angle) * radius;
            float offsetZ = Mathf.Sin(angle) * radius;
            _standingBoardPlayers[i].UpdateVisualsOffset(new(offsetX, 0.0f, offsetZ));
        }
    }
    
    public abstract Task OnPlayerLanded();

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
