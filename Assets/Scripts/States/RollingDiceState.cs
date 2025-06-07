using System.Threading.Tasks;
using UnityEngine;

public class RollingDiceState : GameState
{
    public override async void OnEnter()
    {
        CurrentBoardPlayerController.StartDiceRoll();
        
        await Task.Delay(100);

        PlayerManager.Instance.EnableInput();
        CurrentController.InteractPerformed += OnDiceRollDecided;
        
        async void OnDiceRollDecided()
        {
            bool finished = await CurrentBoardPlayerController.FinishDiceRoll();
            if (!finished) return;

            CurrentController.InteractPerformed -= OnDiceRollDecided;
            await Task.Delay(1000);
            ChangeState(MovingBoardPlayerState);
        }
    }

    public override void OnExit()
    {
    }
}