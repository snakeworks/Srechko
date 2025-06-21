using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "itm_DebtClear", menuName = "Items/Debt Clear", order = 1)]
public class DebtClearItem : Item
{
    public override async Task PerformItemAction()
    {
        var currentData = GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index);
        await Task.Delay(400);
        currentData.AddCoins(currentData.CoinCount * -1);
        AudioManager.Instance.Play(SoundName.BuyItem);
        await BoardManager.Instance.CurrentPlayer.PlayPopupAnimation("DEBT CLEARED!");
        await Task.Delay(700);
    }

    public override bool CanUse()
    {
        return GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index).CoinCount < 0;
    }
}
