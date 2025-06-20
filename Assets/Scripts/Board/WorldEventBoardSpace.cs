using System.Threading.Tasks;

public class WorldEventBoardSpace : BoardSpace
{
    protected override async Task PerformPlayerLanded()
    {
        await BoardManager.Instance.CurrentPlayer.PopupAlert();
        await Task.Delay(800);
        await WorldEventManager.Instance.ApplyRandom();
    }

    protected override Task PerformPlayerPassed() => Task.CompletedTask;
}
