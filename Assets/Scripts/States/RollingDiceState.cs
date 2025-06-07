using System.Threading.Tasks;
using UnityEngine;

public class RollingDiceState : GameState
{
    private bool _canRoll = true;
    
    public override async void OnEnter()
    {
        CurrentBoardPlayerController.StartDiceRoll();

        await Task.Delay(100);

        PlayerManager.Instance.EnableInput();
        CurrentController.InteractPerformed += OnDiceRollDecided;

        async void OnDiceRollDecided()
        {
            if (!_canRoll) return;

            _canRoll = false;
            bool finished = await CurrentBoardPlayerController.FinishDiceRoll();
            if (!finished)
            {
                _canRoll = true;
                return;
            }

            CurrentController.InteractPerformed -= OnDiceRollDecided;
            await Task.Delay(1000);
            ChangeState(MovingBoardPlayerState);
        }
    }

    public override void OnExit()
    {
        _canRoll = true;
    }
}