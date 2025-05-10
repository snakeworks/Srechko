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

    /// <summary>
    /// Open a new current menu and push menu onto the stack.
    /// </summary>
    public static void Push(Menu menu)
    {
        if (menu == null || menu.IsOnStack || _isBufferingMenuOperations || SceneLoader.IsLoading)
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

        Sequence sequence = DOTween.Sequence();
        menu.TweenOpen(sequence);
        menu.EnableInteraction();
        EventSystem.current.SetSelectedGameObject(menu.LastSelectedObject);
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
        
        BufferMenuOperations();


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

        if (IsStackEmpty)
        {
            EventSystem.current.SetSelectedGameObject(null);
            OnEmptyStack?.Invoke();
        }
        else
        {
            CurrentMenu.EnableInteraction();
            EventSystem.current.SetSelectedGameObject(CurrentMenu.LastSelectedObject);
        }
    }

    private static async void BufferMenuOperations()
    {
        _isBufferingMenuOperations = true;
        await Task.Delay((int)(Time.deltaTime * 1500));
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
