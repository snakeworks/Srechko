using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "itm_Mud", menuName = "Items/Mud", order = 1)]
public class MudItem : Item
{
    public int MoveCountModifier = 5;

    public override async Task PerformItemAction()
    {
        BoardSpace space = BoardSpace.GetPredecessor(BoardManager.Instance.CurrentPlayer.StandingOnBoardSpaceId);
        space.SetMudCovered(true, MoveCountModifier);
        await Task.Delay(800);
    }
}
