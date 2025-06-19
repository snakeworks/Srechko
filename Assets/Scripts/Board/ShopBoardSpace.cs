using System.Threading.Tasks;

public class ShopBoardSpace : BoardSpace
{
    protected override Task PerformPlayerLanded() => Task.CompletedTask;

    protected override async Task PerformPlayerPassed()
    {
        PlayerManager.Instance.EnableInput();
        BoardManager.Instance.ShopMenu.Push();
        while (!MenuNavigator.IsStackEmpty)
        {
            await Task.Delay(1);
        }
        PlayerManager.Instance.DisableInput();
    }
}
