using System.Threading.Tasks;
using UnityEngine;

public class ShopBoardSpace : BoardSpace
{
    [SerializeField] private ShopMenu _menuPrefab;

    private ShopMenu _shopMenu;

    protected override void Start()
    {
        base.Start();
        _shopMenu = Instantiate(_menuPrefab, BoardManager.Instance.Canvas.transform).GetComponent<ShopMenu>();
    }

    protected override Task PerformPlayerLanded() => Task.CompletedTask;

    protected override async Task PerformPlayerPassed()
    {
        PlayerManager.Instance.EnableInput();
        _shopMenu.Push();
        while (!MenuNavigator.IsStackEmpty)
        {
            await Task.Delay(1);
        }
        PlayerManager.Instance.DisableInput();
    }
}
