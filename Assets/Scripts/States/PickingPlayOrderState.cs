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
        Stack<int> finalOrder = new();

        // Kinda stupid way to sort the order of the players lol
        // Couldn't think of anything better yet
        // Shouldn't cause too much trouble because there can only be 4 players max
        while (_playerRandomNumbers.Count > 0)
        {
            int highestIndex = 0;
            int highestNumber = 0;
            foreach (var entry in _playerRandomNumbers)
            {
                if (entry.Value > highestNumber)
                {
                    highestIndex = entry.Key;
                    highestNumber = entry.Value;
                    break;
                }
            }
            _playerRandomNumbers.Remove(highestIndex);
            finalOrder.Push(highestIndex);
        }

        BoardManager.Instance.SetTurnOrder(finalOrder.ToList());

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