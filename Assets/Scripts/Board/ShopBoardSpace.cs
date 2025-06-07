using System.Threading.Tasks;

public class ShopBoardSpace : BoardSpace
{
    public async override Task OnPlayerLanded()
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
