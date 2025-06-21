using System.Threading.Tasks;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string Name;
    [TextArea] public string Description;
    public int Price;
    public Sprite Icon;
    public bool RollDiceAfterUse = false;

    public abstract Task PerformItemAction();
}
