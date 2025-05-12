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
        BoardCamera.Instance.TransitionTo(BoardCameraTransforms.PickingOrderView, CameraTransition.Move);
        while (BoardCamera.Instance.IsTransitioning)
        {
            await Task.Yield();
        }
        PlayerManager.Instance.GiveOwnershipToAll();
        PlayerManager.Instance.EnableInput();

        for (int i = 0; i < BoardManager.Instance.BoardPlayerControllerCount; i++)
        {
            var boardPlayer = BoardManager.Instance.GetBoardPlayerControllerAt(i);
            boardPlayer.ShowDiceRolling();
        }

        PlayerManager.Instance.OnAnyPlayerInteractPerformed += OnAnyPlayerInteractPerformed;
    }

    private void OnAnyPlayerInteractPerformed(PlayerController controller)
    {
        if (_playerRandomNumbers.ContainsKey(controller.Index))
        {
            return;
        }
        int number = Random.Range(BoardManager.MinDiceNumber, BoardManager.MaxDiceNumber);
        _playerRandomNumbers.Add(controller.Index, number);
        
        BoardManager.Instance.GetBoardPlayerControllerAt(controller.Index).FinishRollingDice(number, false);

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

        await Task.Delay(1000);

        for (int i = 0; i < BoardManager.Instance.BoardPlayerControllerCount; i++)
        {
            var boardPlayer = BoardManager.Instance.GetBoardPlayerControllerAt(i);
            boardPlayer.HideDice();
        }

        PlayerManager.Instance.DisableInput();
        PlayerManager.Instance.OnAnyPlayerInteractPerformed -= OnAnyPlayerInteractPerformed;
        await Task.Delay(500);
        ChangeState(NextTurnState);
    }

    public override void OnExit()
    {
        PlayerManager.Instance.OnAnyPlayerInteractPerformed -= OnAnyPlayerInteractPerformed;
    }
}