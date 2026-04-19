using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "itm_Sticker", menuName = "Items/Sticker", order = 1)]
public class StickerItem : Item
{
    public override async Task PerformItemAction()
    {
        await BoardManager.Instance.CurrentPlayer.PlayStickerAnimation(Icon);
    }
}
