using System.Threading.Tasks;
using UnityEngine;

public class NormalBoardSpace : BoardSpace
{
    private const int _coinsEarned = 5;

    protected override async Task PerformPlayerLanded()
    {
        var playerData = GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index);
        var boardPlayer = BoardManager.Instance.CurrentPlayer;

        playerData.AddCoins(_coinsEarned);
        await boardPlayer.PlayCoinsAnimation(_coinsEarned);

        await Task.Delay(100);
    }

    protected override Task PerformPlayerPassed() => Task.CompletedTask;
}
