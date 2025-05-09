using System.Collections.Generic;
using System.Linq;

public static class MenuNavigator
{
    /// <summary>
    /// The menu on top of the stack, the one that the player is currently interacting with.
    /// </summary>
    public static Menu CurrentMenu => _menuStack.Peek();

    private static readonly Stack<Menu> _menuStack = new();

    /// <summary>
    /// Open a new current menu and push menu onto the stack.
    /// </summary>
    public static void Push(Menu menu)
    {
        _menuStack.Push(menu);
    }

    /// <summary>
    /// Close the current menu and pop it off the stack.
    /// </summary>
    public static void Pop()
    {
        _menuStack.Pop();
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
