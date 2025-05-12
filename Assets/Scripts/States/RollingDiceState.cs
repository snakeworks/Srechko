using System.Threading.Tasks;
using UnityEngine;

public class RollingDiceState : GameState
{
    public override async void OnEnter()
    {
        CurrentBoardPlayerController.ShowDiceRolling();
        
        await Task.Delay(100);

        CurrentController.InteractPerformed += OnDiceRollDecided;
        
        async void OnDiceRollDecided()
        {
            CurrentController.InteractPerformed -= OnDiceRollDecided;
            int rolledNumber = Random.Range(BoardManager.MinDiceNumber, BoardManager.MaxDiceNumber);
            CurrentBoardPlayerController.FinishRollingDice(rolledNumber);
            await Task.Delay(1000);
            ChangeState(MovingBoardPlayerState);
        }
    }

    public override void OnExit()
    {
    }
}