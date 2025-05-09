using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class MenuNavigator
{
    /// <summary>
    /// The menu on top of the stack, the one that the player is currently interacting with.
    /// </summary>
    public static Menu CurrentMenu => _menuStack.Peek();
    public static bool IsStackEmpty => _menuStack.Count <= 0;

    private static readonly Stack<Menu> _menuStack = new();
    private static bool _isBufferingMenuOperations = false;

    /// <summary>
    /// Open a new current menu and push menu onto the stack.
    /// </summary>
    public static void Push(Menu menu)
    {
        if (menu == null || menu.IsOnStack || _isBufferingMenuOperations)
        {
            return;
        }

        BufferMenuOperations();
        menu.gameObject.SetActive(true);
        menu.TweenOpen(() => {});
        _menuStack.Push(menu);
    }

    /// <summary>
    /// Close the current menu and pop it off the stack.
    /// </summary>
    public static void Pop()
    {
        if (IsStackEmpty || _isBufferingMenuOperations)
        {
            return;
        }

        Menu lastCurrentMenu = CurrentMenu;
        BufferMenuOperations();
        lastCurrentMenu.TweenClose(() =>
        {
            lastCurrentMenu.gameObject.SetActive(false);
        });
        _menuStack.Pop();
    }

    private static async void BufferMenuOperations()
    {
        _isBufferingMenuOperations = true;
        await Task.Delay((int)(Time.deltaTime * 1000));
        _isBufferingMenuOperations = false;
    }

    /// <summary>
    /// Checks if menu is on the stack.
    /// </summary>
    /// <returns>
    /// True, if the menu exists on the stack, otherwise false.
    /// </returns>
    public static bool IsMenuOnStack(Menu menu)
    {
        return _menuStack.Any((m) => m == menu);
    }
}
