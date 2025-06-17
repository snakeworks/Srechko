using System.Threading.Tasks;
using UnityEngine;

public class WorldEventBoardSpace : BoardSpace
{
    public override async Task OnPlayerLanded()
    {
        await WorldEventManager.Instance.ApplyRandom();
    }
}
