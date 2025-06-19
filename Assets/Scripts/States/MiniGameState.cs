using System.Threading.Tasks;
using UnityEngine;

public class MiniGameState : GameState
{
    public override async void OnEnter()
    {
        PlayerManager.Instance.DisableInput();

        await BoardCamera.Instance.TransitionTo(BoardCameraTransforms.FieldOverview, CameraTransition.Move, 0.8f);
        await Task.Delay(200);

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
