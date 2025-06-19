using System.Threading.Tasks;
using UnityEngine;

public class WorldEventBoardSpace : BoardSpace
{
    protected override async Task PerformPlayerLanded()
    {
        await WorldEventManager.Instance.ApplyRandom();
    }

    protected override Task PerformPlayerPassed() => Task.CompletedTask;
}
