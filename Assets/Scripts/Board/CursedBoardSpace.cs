using System.Threading.Tasks;

public class CursedBoardSpace : BoardSpace
{
    private const int _coinsLost = 5;

    protected override async Task PerformPlayerLanded()
    {
        var playerData = GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index);
        var boardPlayer = BoardManager.Instance.CurrentPlayer;

        playerData.RemoveCoins(_coinsLost);
        await boardPlayer.PlayCoinsAnimation(_coinsLost, false);

        await Task.Delay(100);
    }

    protected override Task PerformPlayerPassed() => Task.CompletedTask;
}