using System.Threading.Tasks;

public class CursedBoardSpace : BoardSpace
{
    private const int _coinsLost = 5;

    protected override async Task PerformPlayerLanded()
    {
        var playerData = GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index);
        var boardPlayer = BoardManager.Instance.CurrentPlayer;

        playerData.RemoveCoins(_coinsLost);
        boardPlayer.PlayCoinsAnimation(_coinsLost, false);

        await Task.Delay(1000);
    }

    protected override Task PerformPlayerPassed() => Task.CompletedTask;
}