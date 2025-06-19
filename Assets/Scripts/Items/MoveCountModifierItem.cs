using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "itm_MoveCountModifier", menuName = "Items/Move Count Modifier", order = 1)]
public class MoveCountModifierItem : Item
{
    public int MoveCountModifier = 5;

    public async override Task PerformItemAction()
    {
        BoardManager.Instance.CurrentPlayer.SetMoveCountModifier(MoveCountModifier);
        await BoardManager.Instance.CurrentPlayer.PlayMoveCountAnimation(MoveCountModifier);
        await Task.Delay(100);
    }
}