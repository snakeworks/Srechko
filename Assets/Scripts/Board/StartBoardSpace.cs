using System.Threading.Tasks;
using UnityEngine;

public class StartBoardSpace : BoardSpace
{
    private const int _coinsEarned = 15;

    protected override Task PerformPlayerLanded()
    {
        return Task.CompletedTask;
    }

    protected override async Task PerformPlayerPassed()
    {
        if (BoardManager.Instance.CurrentPlayer.PreviousStandingOnBoardSpaceId < 0)
        {
            return;
        }

        await Awaitable.WaitForSecondsAsync(0.1f);
        GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index).AddCoins(_coinsEarned);
        await BoardManager.Instance.CurrentPlayer.PlayCoinsAnimation(_coinsEarned);
        await Awaitable.WaitForSecondsAsync(0.5f);
    }
}
