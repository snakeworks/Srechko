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
    public bool IsMudCovered { get; private set; } = false;
    public static int Count => _spaces.Count;

    private int _mudMoveCountModifier = 0;
    private readonly List<BoardPlayerController> _standingBoardPlayers = new();
    private static readonly List<BoardSpace> _spaces = new();
    private static bool _sorted = false;

    protected virtual void Awake()
    {
        Id = transform.GetSiblingIndex();
        _spaces.Add(this);
    }

    protected virtual void Start()
    {
        if (_sorted) return;
        _sorted = true;
        _spaces.Sort((space1, space2) => space1.Id.CompareTo(space2.Id));
    }

    public static BoardSpace Get(int id)
    {
        return _spaces[id];
    }

    public static BoardSpace GetRandom()
    {
        return _spaces[Random.Range(0, _spaces.Count)];
    }

    public static BoardSpace GetPredecessor(int id)
    {
        foreach (var space in _spaces)
        {
            if (space.GetNextSpaces().ContainsValue(Get(id)))
            {
                return space;
            }
        }
        return null;
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
            float angle = i * Mathf.PI * 2 / _standingBoardPlayers.Count + Mathf.PI / 4;
            float radius = 0.2f;
            float offsetX = Mathf.Cos(angle) * radius;
            float offsetZ = Mathf.Sin(angle) * radius;
            _standingBoardPlayers[i].UpdateVisualsOffset(new(offsetX, 0.0f, offsetZ));
        }
    }

    public void SetMudCovered(bool covered, int moveCountModifier)
    {
        IsMudCovered = covered;
        if (covered)
        {
            _mudMoveCountModifier = moveCountModifier;
            BoardManager.Instance.CreateMudObjectOnSpace(this);
        }
    }

    public async Task OnPlayerPassed()
    {
        await PerformPlayerPassed();
    }

    protected abstract Task PerformPlayerPassed();

    public async Task OnPlayerLanded()
    {
        if (IsMudCovered)
        {
            BoardManager.Instance.CurrentPlayer
                .SetMoveCountModifier(_mudMoveCountModifier > 0 ? -_mudMoveCountModifier : _mudMoveCountModifier);
            await BoardManager.Instance.CurrentPlayer
                .PlayMoveCountAnimation(_mudMoveCountModifier, false);
            await Task.Delay(100);
        }
        await PerformPlayerLanded();
    }

    protected abstract Task PerformPlayerLanded();

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
