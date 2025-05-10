using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

/// <summary>
/// Handles player input for a specific device.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public int Index => transform.GetSiblingIndex();
    public bool IsInputEnabled => _playerInput.inputIsActive;
    public InputDevice Device => _playerInput.devices[0]; // TODO: Could cause problem in the future???
    public event Action InteractPerformed;
    public event Action OpenPauseMenuPerformed;
    public event Action CancelPerformed;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void TryPerform(InputAction.CallbackContext context, Action action)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            action?.Invoke();
        }
    }

    public void GiveOwnership()
    {
        _playerInput.ActivateInput();
        InputSystem.EnableDevice(Device);
        CancelPerformed += PopMenu;
    }

    public void RevokeOwnership()
    {
        _playerInput.DeactivateInput();
        InputSystem.DisableDevice(Device); 
        CancelPerformed -= PopMenu;
    }

    private void PopMenu()
    {
        MenuNavigator.Pop(this);
    }

    public void InputInteract(InputAction.CallbackContext context) => TryPerform(context, InteractPerformed);
    public void InputOpenPauseMenu(InputAction.CallbackContext context) => TryPerform(context, OpenPauseMenuPerformed);
    public void InputCancel(InputAction.CallbackContext context) => TryPerform(context, CancelPerformed);
}
