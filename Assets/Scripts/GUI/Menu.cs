using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public abstract class Menu : MonoBehaviour
{
    public bool IsCurrent => MenuNavigator.CurrentMenu == this;
    public bool IsOnStack => MenuNavigator.IsMenuOnStack(this);
    public GameObject LastSelectedObject { get; set; }
    
    protected CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        LastSelectedObject = GetComponentInChildren<Button>().gameObject;
        DisableInteraction();
        
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

    public void EnableInteraction()
    {
        _canvasGroup.interactable = true;
    }

    public void DisableInteraction()
    {
        _canvasGroup.interactable = false;
    }
}