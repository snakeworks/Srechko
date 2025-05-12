using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PickingPlayerOrderState : GameState
{
    private readonly Dictionary<PlayerController, int> _playerRandomNumbers = new();

    public override async void OnEnter()
    {
        PlayerManager.Instance.DisableInput();
        BoardCamera.Instance.TransitionTo(BoardCameraTransforms.PickingOrderView, CameraTransition.Move);
        while (BoardCamera.Instance.IsTransitioning)
        {
            await Task.Yield();
        }
        PlayerManager.Instance.GiveOwnershipToAll();
        PlayerManager.Instance.OnAnyPlayerInteractPerformed += OnAnyPlayerInteractPerformed;
    }

    private void OnAnyPlayerInteractPerformed(PlayerController controller)
    {
        if (_playerRandomNumbers.ContainsKey(controller))
        {
            return;
        }
        int number = Random.Range(BoardManager.MinDiceNumber, BoardManager.MaxDiceNumber);
        _playerRandomNumbers.Add(controller, number);
        
        // Finished picking random numbers for all players
        if (_playerRandomNumbers.Count >= PlayerManager.Instance.ControllerCount)
        {
            FinishPickingProcess();
        }
    }

    private async void FinishPickingProcess()
    {
        PlayerManager.Instance.DisableInput();
        PlayerManager.Instance.OnAnyPlayerInteractPerformed -= OnAnyPlayerInteractPerformed;
        await Task.Delay(500);
        ChangeState(ChoosingBoardActionState);
    }

    public override void OnExit()
    {
        PlayerManager.Instance.OnAnyPlayerInteractPerformed -= OnAnyPlayerInteractPerformed;
    }
}