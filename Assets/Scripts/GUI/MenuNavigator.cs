using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public static class MenuNavigator
{
    /// <summary>
    /// The menu on top of the stack, the one that the player is currently interacting with.
    /// </summary>
    public static Menu CurrentMenu => _menuStack.Peek();
    public static event Action OnEmptyStack;
    public static bool IsStackEmpty => _menuStack.Count <= 0;

    private static readonly Stack<Menu> _menuStack = new();
    private static bool _isBufferingMenuOperations = false;

    public static Stack<Menu> GetStack()
    {
        return _menuStack;
    }

    /// <summary>
    /// Opens a new menu and pushes it onto the stack.
    /// </summary>
    public static void Push(Menu menu)
    {
        if (menu == null || menu.IsOnStack || _isBufferingMenuOperations)
        {
            return;
        }

        BufferMenuOperations();

        if (!IsStackEmpty)
        {
            CurrentMenu.DisableInteraction();
            CurrentMenu.LastSelectedObject = EventSystem.current.currentSelectedGameObject;
        }

        menu.gameObject.SetActive(true);

        if (menu.PauseGameOnOpen)
        {
            Pauser.PauseGame();
        }

        Sequence sequence = DOTween.Sequence();
        menu.TweenOpen(sequence);
        menu.EnableInteraction();
        _menuStack.Push(menu);
        BufferSelectButton();
    }

    /// <summary>
    /// Pops the current menu on top of the stack and replaces it with the new <c>menu</c>.
    /// </summary>
    public static void PushReplacement(Menu menu)
    {
        Pop(asReplacement: true);
        Push(menu);
    }

    /// <summary>
    /// Closes the current menu and pops it off the stack.
    /// </summary>
    public static void Pop(bool asReplacement = false)
    {
        if (IsStackEmpty || _isBufferingMenuOperations || !CurrentMenu.CanPop)
        {
            return;
        }
        
        if (!asReplacement)
        {
            BufferMenuOperations();
        }

        Menu lastCurrentMenu = CurrentMenu;
        Sequence sequence = DOTween.Sequence();
        sequence.OnComplete(() =>
        {
            lastCurrentMenu.gameObject.SetActive(false);
        });

        lastCurrentMenu.LastSelectedObject = EventSystem.current.currentSelectedGameObject;
        lastCurrentMenu.TweenClose(sequence);        
        lastCurrentMenu.DisableInteraction();
        _menuStack.Pop();

        if (asReplacement)
        {
            return;
        }

        if (IsStackEmpty)
        {
            EventSystem.current.SetSelectedGameObject(null);
            Pauser.UnpauseGame();
            OnEmptyStack?.Invoke();
        }
        else
        {
            CurrentMenu.EnableInteraction();
            BufferSelectButton();
        }
    }

    /// <summary>
    /// Forcefully closes all menus on the stack and empties it.
    /// </summary>
    public static void Clear()
    {
        if (IsStackEmpty)
        {
            return;
        }

        Sequence sequence = DOTween.Sequence();

        foreach (var menu in _menuStack)
        {
            menu.TweenClose(sequence);
        }
        sequence.Complete();
        foreach (var menu in _menuStack)
        {
            menu.gameObject.SetActive(false);
        }

        Pauser.UnpauseGame();

        _menuStack.Clear();
        OnEmptyStack?.Invoke();
    }

    private static async void BufferMenuOperations()
    {
        _isBufferingMenuOperations = true;
        await Task.Delay((int)(Time.deltaTime * 1500));
        _isBufferingMenuOperations = false;
    }

    private static async void BufferSelectButton()
    {
        await Task.Delay((int)(Time.deltaTime * 1500));
        EventSystem.current.SetSelectedGameObject(CurrentMenu.LastSelectedObject);
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
