using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "itm_Blocker", menuName = "Items/Blocker", order = 1)]
public class BlockerItem : Item
{
    public override async Task PerformItemAction()
    {
        var selectedPlayer = await BoardManager.Instance.BoardPlayerSelectionMenu.GetSelectedPlayer();
        await BoardCamera.Instance.TransitionToPlayer(selectedPlayer.Index);
        selectedPlayer.SetCanUseItems(false);
        AudioManager.Instance.Play(SoundName.DuctTape);
        await selectedPlayer.PlayTapeAnimation();
        await Task.Delay(1000);
    }
}
