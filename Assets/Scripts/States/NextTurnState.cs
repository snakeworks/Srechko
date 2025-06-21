using System.Threading.Tasks;

public class NextTurnState : GameState
{
    public override async void OnEnter()
    {
        PlayerManager.Instance.DisableInput();

        if (BoardManager.Instance.CurrentPlayer != null && !BoardManager.Instance.CurrentPlayer.SkipNextTurn)
        {
            BoardManager.Instance.CurrentPlayer.OnTurnEnd();
        }
        BoardManager.Instance.NextTurn();

        await Task.Delay(500);

        if (BoardManager.Instance.CurrentRound > BoardManager.MaxRoundCount)
        {
            ChangeState(EndState);
            return;
        }

        if (!AudioManager.Instance.IsPlaying(SoundName.BoardTheme))
        {
            AudioManager.Instance.Play(SoundName.BoardTheme);
        }

        if (BoardManager.Instance.CurrentPlayerTurnIndex == -1)
        {
            ChangeState(MiniGameState);
        }
        else
        {
            ChangeState(ChoosingBoardActionState);
        }
    }

    public override void OnExit()
    {
    }
}
