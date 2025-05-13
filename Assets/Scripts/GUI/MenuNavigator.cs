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
    public static bool IsBufferingMenuOperations { get; private set; } = false;

    private static readonly Stack<Menu> _menuStack = new();

    public static Stack<Menu> GetStack()
    {
        return _menuStack;
    }

    /// <summary>
    /// Opens the new <c>menu</c> and pushes it onto the stack.
    /// </summary>
    public static void Push(Menu menu)
    {
        if (menu == null || menu.IsOnStack || IsBufferingMenuOperations)
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
        sequence.OnComplete(() => 
        {
            menu.IsTweening = false;
        });
        menu.IsTweening = true;
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
        Pop(PopMode.Replacement);
        Push(menu);
    }

    /// <summary>
    /// Closes the current menu and pops it off the stack.
    /// </summary>
    public static void Pop(PopMode mode = PopMode.Default)
    {
        if (IsStackEmpty || IsBufferingMenuOperations || !CurrentMenu.CanPop)
        {
            return;
        }
        
        if (mode == PopMode.Default)
        {
            BufferMenuOperations();
        }

        Menu lastCurrentMenu = CurrentMenu;
        Sequence sequence = DOTween.Sequence();
        sequence.OnComplete(() =>
        {
            if (!lastCurrentMenu.IsOnStack)
            {
                lastCurrentMenu.gameObject.SetActive(false);
            }
            lastCurrentMenu.IsTweening = false;
        });

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            lastCurrentMenu.LastSelectedObject = EventSystem.current.currentSelectedGameObject;
        }
        lastCurrentMenu.IsTweening = true;
        lastCurrentMenu.TweenClose(sequence);        
        lastCurrentMenu.DisableInteraction();
        _menuStack.Pop();

        if (mode == PopMode.Replacement)
        {
            return;
        }

        if (IsStackEmpty)
        {
            EventSystem.current.SetSelectedGameObject(null);
            Pauser.UnpauseGame();
            OnEmptyStack?.Invoke();
        }
        else if (mode == PopMode.Default)
        {
            CurrentMenu.EnableInteraction();
            BufferSelectButton();
        }
        else if (mode == PopMode.ForceAll)
        {
            EventSystem.current.SetSelectedGameObject(CurrentMenu.LastSelectedObject);
        }
    }

    /// <summary>
    /// Pops the menu on top of the stack. Ignores <c>CanPop</c> property.
    /// </summary>
    public static void ForcePop(PopMode mode = PopMode.Default)
    {
        if (IsStackEmpty)
        {
            return;
        }
        bool previousCanPopValue = CurrentMenu.CanPop;
        Menu lastCurrentMenu = CurrentMenu;
        lastCurrentMenu.CanPop = true;
        Pop(mode);
        lastCurrentMenu.CanPop = previousCanPopValue;
    }

    /// <summary>
    /// Pops every menu off the stack. Ignores <c>CanPop</c> property.
    /// </summary>
    public static void ForcePopUntilEmpty()
    {
        while (!IsStackEmpty)
        {
            ForcePop(PopMode.ForceAll);
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
        IsBufferingMenuOperations = true;
        await Task.Delay((int)(Time.deltaTime * 2500));
        IsBufferingMenuOperations = false;
    }

    private static async void BufferSelectButton()
    {
        await Task.Delay((int)(Time.deltaTime * 2500));
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

    public enum PopMode
    {
        Default,
        Replacement,
        ForceAll
    }
}
