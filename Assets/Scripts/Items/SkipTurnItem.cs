using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "itm_SkipTurn", menuName = "Items/Skip Turn", order = 1)]
public class SkipTurnItem : Item
{
    public override async Task PerformItemAction()
    {
        var selectedPlayer = await BoardManager.Instance.BoardPlayerSelectionMenu.GetSelectedPlayer();
        selectedPlayer.SetSkipNextTurn(true);
        await BoardCamera.Instance.TransitionToPlayer(selectedPlayer.Index);
        await selectedPlayer.PlayPopupAnimation($"SKIPPING NEXT TURN");
    }
}
