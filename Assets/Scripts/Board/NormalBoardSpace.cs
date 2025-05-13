using System.Threading.Tasks;
using UnityEngine;

public class NormalBoardSpace : BoardSpace
{
    private const int _coinsEarned = 5;

    public override async Task OnPlayerLanded()
    {
        var playerData = GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index);
        var boardPlayer = BoardManager.Instance.CurrentPlayer;

        playerData.AddCoins(_coinsEarned);
        boardPlayer.PlayCoinsGet(_coinsEarned);

        await Task.Delay(1000);
    }
}
