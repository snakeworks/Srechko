using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState CurrentState { get; private set; }

    private readonly List<BoardPlayerData> _boardPlayerData = new();

    protected override void Init()
    {   
        for (int i = 0; i < PlayerManager.Instance.ControllerCount; i++)
        {
            var child = new GameObject($"PlayerData{i+1}");
            child.transform.SetParent(transform);
            child.transform.position = Vector3.zero;
            _boardPlayerData.Add(child.AddComponent<BoardPlayerData>());
        }
        GetBoardPlayerData(0).AddCoins(6000);
        GetBoardPlayerData(1).AddCoins(6000);
        SceneLoader.OnSceneLoadFinish += EvaluateGameState;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneLoader.OnSceneLoadFinish -= EvaluateGameState;
    }

    private void Start()
    {
        BoardManager.Instance.StartFromScratch();
        ChangeState(GameState.StartingState);
    }

    public void ChangeState(GameState newState)
    {
        if (newState == null)
        {
            return;
        }
        CurrentState?.OnExit();
        CurrentState = newState;
        CurrentState.OnEnter();
    }

    public BoardPlayerData GetBoardPlayerData(int index)
    {
        if (!_boardPlayerData.InRange(index))
        {
            return null;
        }
        return _boardPlayerData[index];
    }

    private void EvaluateGameState()
    {
        if (SceneLoader.ActiveScene == Scene.Board)
        {

        }
        else if (SceneLoader.IsActiveSceneMinigame)
        {

        }
        else
        {
            Destroy(this);
        }
    }
}
