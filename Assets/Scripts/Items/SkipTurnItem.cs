using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "itm_SkipTurn", menuName = "Items/Skip Turn", order = 1)]
public class SkipTurnItem : Item
{
    public override async Task PerformItemAction()
    {
        var selectedPlayer = await BoardManager.Instance.BoardPlayerSelectionMenu.GetSelectedPlayer();
        await BoardCamera.Instance.TransitionToPlayer(selectedPlayer.Index);
        selectedPlayer.SetSkipNextTurn(true);
        AudioManager.Instance.Play(SoundName.Sleep);
        await Task.Delay(1000);
    }
}
