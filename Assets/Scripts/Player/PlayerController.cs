using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player input for a specific device.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public int Index => transform.GetSiblingIndex();
    public bool IsInputEnabled => PlayerInput.inputIsActive;
    public InputActionAsset ActionsAsset => PlayerInput.actions;
    public InputDevice Device
    {
        get
        {
            if (_device == null)
            {
                _device = PlayerInput.devices[0];
            }
            return _device;
        }
    }
    public event Action InteractPerformed;
    public event Action OpenPauseMenuPerformed;
    public event Action CancelPerformed;

    // Doing this convoluted ass work around because of some stupid ass Unity reason.
    // For future reference: Always make calls to PlayerInput and not _playerInput.
    private PlayerInput PlayerInput
    {
        get
        {
            if (_playerInput == null)
            {
                _playerInput = GetComponent<PlayerInput>();
            }
            return _playerInput;
        }
    }
    private PlayerInput _playerInput;
    private InputDevice _device;

    private void TryPerform(InputAction.CallbackContext context, Action action)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            action?.Invoke();
        }
    }

    public void EnableInput()
    {
        PlayerInput.ActivateInput();
        InputSystem.EnableDevice(Device);
        CancelPerformed += PopMenu;
    }

    public void DisableInput()
    {
        PlayerInput.DeactivateInput();
        InputSystem.DisableDevice(Device);
        CancelPerformed -= PopMenu;
    }

    private void PopMenu()
    {
        MenuNavigator.Pop();
    }

    private void OnDestroy()
    {
        // Returning control back to the device after this player has left the game
        InputSystem.EnableDevice(Device);
    }

    public void InputInteract(InputAction.CallbackContext context) => TryPerform(context, InteractPerformed);
    public void InputOpenPauseMenu(InputAction.CallbackContext context) => TryPerform(context, OpenPauseMenuPerformed);
    public void InputCancel(InputAction.CallbackContext context) => TryPerform(context, CancelPerformed);
}
