using System;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    public bool IsCurrent => MenuNavigator.CurrentMenu == this;
    public bool IsOnStack => MenuNavigator.IsMenuOnStack(this);

    private void Awake()
    {
        Init();
        gameObject.SetActive(false);
    }

    protected abstract void Init();

    /// <summary>
    /// Opening tween animation for the menu. Called when the menu is pushed onto the stack. 
    /// Do NOT call this manually.
    /// </summary>
    public abstract void TweenOpen(Action onComplete);
    
    /// <summary>
    /// Closing tween animation for the menu. Called when the menu is popped off the stack. 
    /// Do NOT call this manually.
    /// </summary>
    public abstract void TweenClose(Action onComplete);
}