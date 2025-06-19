using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BoardManager : StaticInstance<BoardManager>
{
    [SerializeField] private BoardPlayerController _boardPlayerControllerPrefab;
    [SerializeField] private Transform[] _startingPositions;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private BoardActionMenu _boardActionMenu;
    [SerializeField] private BoardPlayerSelectionMenu _boardPlayerSelectionMenu;
    [SerializeField] private BoardSpace _startingSpace;
    [SerializeField] private GameObject _mudPrefab;

    public int CurrentPlayerTurnIndex { get; private set; } = -1;
    public int BoardPlayerControllerCount => _boardPlayerControllers.Count;
    public BoardPlayerController CurrentPlayer => GetBoardPlayerControllerAt(_boardPlayerIndexOrder[CurrentPlayerTurnIndex]);
    public Canvas Canvas => _canvas;
    public BoardActionMenu BoardActionMenu => _boardActionMenu;
    public BoardPlayerSelectionMenu BoardPlayerSelectionMenu => _boardPlayerSelectionMenu;
    public BoardSpace StartingSpace => _startingSpace;
    public List<int> BoardPlayerIndexOrder => _boardPlayerIndexOrder;
    public event Action OnNextTurn;
    public event Action OnTurnOrderSet;
    public const int MinDiceNumber = 1;
    public const int MaxDiceNumber = 15;

    private readonly List<BoardPlayerController> _boardPlayerControllers = new();
    private List<int> _boardPlayerIndexOrder;

    private void InitBoard()
    {
        // Creating all of the board player controllers from a prefab
        for (int i = 0; i < PlayerManager.Instance.ControllerCount; i++)
        {
            var boardPlayerObject = Instantiate(_boardPlayerControllerPrefab, transform);
            boardPlayerObject.name = $"BoardPlayerController{i + 1}";
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

    public void CreateMudObjectOnSpace(BoardSpace space)
    {
        var obj = Instantiate(_mudPrefab);
        obj.transform.SetParent(space.transform);
        obj.transform.localPosition = new(0.0f, 0.0f, -0.01f);
        obj.transform.localScale = Vector3.zero;
        obj.transform.DOScale(1.0f, 0.4f);
    }
}
