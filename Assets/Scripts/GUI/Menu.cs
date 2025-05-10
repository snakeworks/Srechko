using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public abstract class Menu : MonoBehaviour
{
    public bool OpenImmediate = false;
    public bool CanPop = true;
    public bool IsCurrent => MenuNavigator.CurrentMenu == this;
    public bool IsOnStack => MenuNavigator.IsMenuOnStack(this);
    public GameObject LastSelectedObject { get; set; }
    
    protected CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        Button firstButton = GetComponentInChildren<Button>();
        LastSelectedObject = firstButton == null ? null : firstButton.gameObject;
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
}