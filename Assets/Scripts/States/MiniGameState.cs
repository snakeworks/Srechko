using System.Threading.Tasks;
using UnityEngine;

public class MiniGameState : GameState
{
    public override void OnEnter()
    {
        PlayerManager.Instance.DisableInput();
        MiniGameManager.Instance.BeginRandom();
        MiniGameManager.Instance.Finished += OnFinished;

        void OnFinished()
        {
            MiniGameManager.Instance.Finished -= OnFinished;
            ChangeState(NextTurnState);
        }
    }

    public override void OnExit()
    {
    }
}
