using System.Threading.Tasks;

public class CursedBoardSpace : BoardSpace
{
    private const int _coinsLost = 5;

    public async override Task OnPlayerLanded()
    {
        var playerData = GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index);
        var boardPlayer = BoardManager.Instance.CurrentPlayer;

        playerData.RemoveCoins(_coinsLost);
        boardPlayer.PlayCoinsAnimation(_coinsLost, false);

        await Task.Delay(1000);
    }
}