using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : StaticInstance<BoardManager>
{
    [SerializeField] private BoardPlayerController _boardPlayerControllerPrefab;
    [SerializeField] private Transform[] _startingPositions;
    [SerializeField] private BoardActionMenu _boardActionMenu;
    [SerializeField] private ShopMenu _shopMenu;
    [SerializeField] private BoardSpace _startingSpace;

    public int CurrentPlayerTurnIndex { get; private set; } = -1;
    public int BoardPlayerControllerCount => _boardPlayerControllers.Count;
    public BoardPlayerController CurrentPlayer => GetBoardPlayerControllerAt(_boardPlayerIndexOrder[CurrentPlayerTurnIndex]);
    public BoardActionMenu BoardActionMenu => _boardActionMenu;
    public ShopMenu ShopMenu => _shopMenu; 
    public BoardSpace StartingSpace => _startingSpace;
    public List<int> BoardPlayerIndexOrder => _boardPlayerIndexOrder;
    public event Action OnNextTurn;
    public event Action OnTurnOrderSet;
    public const int MinDiceNumber = 1;
    public const int MaxDiceNumber = 10;

    private readonly List<BoardPlayerController> _boardPlayerControllers = new();
    private List<int> _boardPlayerIndexOrder;

    private void InitBoard()
    {
        // Creating all of the board player controllers from a prefab
        for (int i = 0; i < PlayerManager.Instance.ControllerCount; i++)
        {
            var boardPlayerObject = Instantiate(_boardPlayerControllerPrefab, transform);
            boardPlayerObject.name = $"BoardPlayerController{i+1}";
            _boardPlayerControllers.Add(boardPlayerObject.GetComponent<BoardPlayerController>());
        }
    }

    /// <summary>
    /// Creates the board and the players on it from scratch.
    /// </summary>
    public void StartFromScratch()
    {
        InitBoard();
        for (int i = 0; i < PlayerManager.Instance.ControllerCount; i++)
        {
            var boardPlayerController = GetBoardPlayerControllerAt(i);
            boardPlayerController.transform.position = _startingPositions[i].position;
        }
    }

    /// <summary>
    /// Creates the board and the players on it based on a <c>BoardSnapshot</c>.
    /// A snapshot of the board is taken right before the scene is about to change
    /// to a different one that is not the board scene. This method is usually called 
    /// when the players are done with a minigame. 
    /// </summary>
    public void StartFromSnapshot(BoardSnapshot snapshot)
    {
        InitBoard();
        _boardPlayerIndexOrder = snapshot.BoardPlayerIndexOrder;
        for (int i = 0; i < _boardPlayerIndexOrder.Count; i++)
        {
            var boardPlayerController = GetBoardPlayerControllerAt(i);
            // TODO: Set position, etc.
        }
    }

    public BoardPlayerController GetBoardPlayerControllerAt(int index)
    {
        if (!_boardPlayerControllers.InRange(index))
        {
            return null;
        }
        return _boardPlayerControllers[index];
    }

    public BoardSnapshot GetBoardSnapshot()
    {
        return new BoardSnapshot(
            CurrentPlayerTurnIndex,
            _boardPlayerIndexOrder
        );
    }

    public void SetTurnOrder(List<int> order)
    {
        _boardPlayerIndexOrder = order;
        OnTurnOrderSet?.Invoke();
    }

    public void NextTurn()
    {
        CurrentPlayerTurnIndex++;
        if (CurrentPlayerTurnIndex >= PlayerManager.Instance.ControllerCount)
        {
            // Setting to -1 for the beginning of a minigame
            CurrentPlayerTurnIndex = -1;
        }
        OnNextTurn?.Invoke();
    }
}
