using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public State CurrentState { get; private set; } = State.Idle;    
    public BoardPlayer CurrentPlayer => _boardPlayerOrder[_currentPlayerTurnIndex];

    private int _currentPlayerTurnIndex = 0;
    private readonly List<BoardPlayer> _boardPlayerOrder = new();

    protected override void Init()
    {
        SceneLoader.OnSceneLoadFinish += EvaluateGameState;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneLoader.OnSceneLoadFinish -= EvaluateGameState;
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
            Debug.LogError($"Unhandled scene during gameplay: '{SceneLoader.ActiveScene}'.");
        }

        StartCoroutine(Delay());
        IEnumerator Delay()
        {
            while (SceneLoader.IsLoading)
            {
                yield return null;
            }
        }
    }


    public enum State
    {
        Idle,
        PickingPlayOrder,
        PlayerPlaying,
        DoingMinigame
    }
}
