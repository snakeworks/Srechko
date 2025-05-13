using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public abstract class Menu : MonoBehaviour
{
    public bool OpenImmediate = false;
    public bool CanPop = true;
    public bool PauseGameOnOpen = false;
    public bool RememberLastSelected = true;
    public bool IsCurrent => !MenuNavigator.IsStackEmpty && MenuNavigator.CurrentMenu == this;
    public bool IsOnStack => MenuNavigator.IsMenuOnStack(this);
    public bool IsTweening { get; set; } = false;
    public GameObject LastSelectedObject { get; set; }
    
    protected CanvasGroup _canvasGroup;
    private Button _firstSelectedButton;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        _firstSelectedButton = GetComponentInChildren<Button>();
        LastSelectedObject = _firstSelectedButton == null ? null : _firstSelectedButton.gameObject;
        DisableInteraction();
        
        Init();
        
        if (OpenImmediate)
        {
            MenuNavigator.Push(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    protected abstract void Init();

    /// <summary>
    /// Opening tween animation for the menu. Called when the menu is pushed onto the stack. 
    /// Do NOT call this manually.
    /// </summary>
    /// <param name="sequence">The sequence that the tweens must be appended or inserted into.</param>
    public abstract void TweenOpen(Sequence sequence);
    
    /// <summary>
    /// Closing tween animation for the menu. Called when the menu is popped off the stack. 
    /// Do NOT call this manually.
    /// </summary>
    /// <param name="sequence">The sequence that the tweens must be appended or inserted into.</param>
    public abstract void TweenClose(Sequence sequence);

    public void EnableInteraction()
    {
        _canvasGroup.interactable = true;
    }

    public void DisableInteraction()
    {
        _canvasGroup.interactable = false;
    }

    /// <summary>
    /// Pushes this menu onto the stack. This is the same as calling: <c>MenuNavigator.Push(this)</c>.
    /// </summary>
    public void Push()
    {
        MenuNavigator.Push(this);
    }

    /// <summary>
    /// Pops this menu off the stack if it is on top of the stack.
    /// </summary>
    public void Pop()
    {
        if (!MenuNavigator.IsStackEmpty && MenuNavigator.CurrentMenu == this)
        {
            MenuNavigator.Pop();
        }
    }

    /// <summary>
    /// Pops this menu off the stack if it is on top of the stack.
    /// This method ignores the <c>CanPop</c> property.
    /// </summary>
    public void ForcePop()
    {
        if (!MenuNavigator.IsStackEmpty && MenuNavigator.CurrentMenu == this)
        {
            MenuNavigator.ForcePop();
        }
    }

    /// <summary>
    /// Resets the last selected object to be the first button on the menu.
    /// </summary>
    public void ResetLastSelectedObject()
    {
        LastSelectedObject = _firstSelectedButton.gameObject;
    }
}