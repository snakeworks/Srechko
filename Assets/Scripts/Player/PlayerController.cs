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
    public event Action PromptSouthPerformed;
    public event Action PromptEastPerformed;
    public event Action PromptWestPerformed;
    public event Action PromptNorthPerformed;
    public event Action OpenPauseMenuPerformed;
    public event Action CancelPerformed;
    public event Action<Vector2> MovePerformed;
    public event Action OpenDevMenuPerformed;

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

    public void InputMove(InputAction.CallbackContext context) => MovePerformed?.Invoke(context.ReadValue<Vector2>());
    public void InputInteract(InputAction.CallbackContext context) => TryPerform(context, InteractPerformed);
    public void InputPromptSouth(InputAction.CallbackContext context) => TryPerform(context, PromptSouthPerformed);
    public void InputPromptEast(InputAction.CallbackContext context) => TryPerform(context, PromptEastPerformed);
    public void InputPromptWest(InputAction.CallbackContext context) => TryPerform(context, PromptWestPerformed);
    public void InputPromptNorth(InputAction.CallbackContext context) => TryPerform(context, PromptNorthPerformed);
    public void InputOpenPauseMenu(InputAction.CallbackContext context) => TryPerform(context, OpenPauseMenuPerformed);
    public void InputCancel(InputAction.CallbackContext context) => TryPerform(context, CancelPerformed);
    public void InputOpenDevMenu(InputAction.CallbackContext context) => TryPerform(context, OpenDevMenuPerformed);
}
