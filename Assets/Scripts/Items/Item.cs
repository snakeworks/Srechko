using System.Threading.Tasks;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string Name;
    public int InventoryMaxCount = 3;
    public int ShopMinCount = 1;
    public int ShopMaxCount = 3;
    public bool RollDiceAfterUse = false;
    public int Price;
    [TextArea] public string Description;
    public Sprite Icon;

    public abstract Task PerformItemAction();
    public virtual bool CanUse() { return true; }
}
