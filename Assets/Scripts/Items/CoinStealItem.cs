using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "itm_CoinSteal", menuName = "Items/Coin Steal", order = 1)]
public class CoinStealItem : Item
{
    public int CoinStealCount = 5;

    public override async Task PerformItemAction()
    {
        var selectedPlayer = await BoardManager.Instance.BoardPlayerSelectionMenu.GetSelectedPlayer();
        
        await BoardCamera.Instance.TransitionToPlayer(selectedPlayer.Index);
        GameManager.Instance.GetBoardPlayerData(selectedPlayer.Index).RemoveCoins(CoinStealCount);
        await selectedPlayer.PlayCoinsAnimation(CoinStealCount, false);

        await BoardCamera.Instance.TransitionToPlayer(BoardManager.Instance.CurrentPlayer.Index);
        GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index).AddCoins(CoinStealCount);
        await BoardManager.Instance.CurrentPlayer.PlayCoinsAnimation(CoinStealCount);
    }
}
