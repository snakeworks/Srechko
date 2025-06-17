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

    public abstract Task OnPlayerLanded();

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
