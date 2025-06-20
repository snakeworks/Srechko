using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PickingPlayerOrderState : GameState
{
    private readonly Dictionary<int, int> _playerRandomNumbers = new();

    public override async void OnEnter()
    {
        PlayerManager.Instance.DisableInput();
        
        await BoardCamera.Instance.TransitionTo(BoardCameraTransforms.PickingOrderView, CameraTransition.Move, 1.5f);

        PlayerManager.Instance.GiveOwnershipToAll();
        PlayerManager.Instance.EnableInput();

        for (int i = 0; i < BoardManager.Instance.BoardPlayerControllerCount; i++)
        {
            var boardPlayer = BoardManager.Instance.GetBoardPlayerControllerAt(i);
            boardPlayer.StartDiceRoll();
        }

        PlayerManager.Instance.OnAnyPlayerInteractPerformed += OnAnyPlayerInteractPerformed;
    }

    private async void OnAnyPlayerInteractPerformed(PlayerController controller)
    {
        // TODO: This will likely not work with multiple die, however that shouldn't be a
        // problem because all players have one dice at the beginning of the game.
        var playerController = BoardManager.Instance.GetBoardPlayerControllerAt(controller.Index);
        if (_playerRandomNumbers.ContainsKey(controller.Index) || playerController.LastRolledDiceNumber > 0)
        {
            return;
        }
        
        bool result = await BoardManager.Instance.GetBoardPlayerControllerAt(controller.Index).FinishDiceRoll();

        if (result == false) return;

        _playerRandomNumbers.Add(controller.Index, playerController.LastRolledDiceNumber);

        // Finished picking random numbers for all players
        if (_playerRandomNumbers.Count >= PlayerManager.Instance.ControllerCount)
        {
            FinishPickingProcess();
        }
    }

    private async void FinishPickingProcess()
    {
        List<int> finalOrder = new();

        finalOrder = _playerRandomNumbers
            .OrderByDescending(pair => pair.Value)
            .Select(pair => pair.Key)
            .ToList();

        BoardManager.Instance.SetTurnOrder(finalOrder);

        await Task.Delay(800);

        for (int i = 0; i < BoardManager.Instance.BoardPlayerControllerCount; i++)
        {
            var boardPlayer = BoardManager.Instance.GetBoardPlayerControllerAt(i);
            boardPlayer.HideFinalDiceNumber();
        }

        PlayerManager.Instance.DisableInput();
        PlayerManager.Instance.OnAnyPlayerInteractPerformed -= OnAnyPlayerInteractPerformed;
        
        await Task.Delay(100);
        
        ChangeState(NextTurnState);
    }

    public override void OnExit()
    {
        PlayerManager.Instance.OnAnyPlayerInteractPerformed -= OnAnyPlayerInteractPerformed;
    }
}