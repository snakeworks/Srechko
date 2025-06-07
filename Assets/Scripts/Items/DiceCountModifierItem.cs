using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "itm_DiceCountModifier", menuName = "Items/Dice Count Modifier", order = 1)]
public class DiceCountModifierItem : Item
{
    public int DiceCountModifier = 1;

    public async override Task PerformItemAction()
    {
        BoardManager.Instance.CurrentPlayer.SetDiceCount(DiceCountModifier);
        await Task.Delay(100);
    }
}