using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player input for a specific device.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public int Index => transform.GetSiblingIndex();
    public bool IsInputEnabled => PlayerInput.inputIsActive;
    public InputDevice Device => PlayerInput.devices[0]; // TODO: Could cause problem in the future???
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

    private void TryPerform(InputAction.CallbackContext context, Action action)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            action?.Invoke();
        }
    }

    public void EnableInput(Dictionary<PlayerController, InputDevice> disabledInputDevices)
    {
        if (disabledInputDevices.ContainsKey(this))
        {
            disabledInputDevices.Remove(this);
        }
        PlayerInput.ActivateInput();
        InputSystem.EnableDevice(Device);
        CancelPerformed += PopMenu;
    }

    public void DisableInput(Dictionary<PlayerController, InputDevice> disabledInputDevices)
    {
        if (!disabledInputDevices.ContainsKey(this))
        {
            disabledInputDevices.Add(this, Device);
        }
        PlayerInput.DeactivateInput();
        InputSystem.DisableDevice(Device);
        CancelPerformed -= PopMenu;
    }

    private void PopMenu()
    {
        MenuNavigator.Pop();
    }

    public void InputInteract(InputAction.CallbackContext context) => TryPerform(context, InteractPerformed);
    public void InputOpenPauseMenu(InputAction.CallbackContext context) => TryPerform(context, OpenPauseMenuPerformed);
    public void InputCancel(InputAction.CallbackContext context) => TryPerform(context, CancelPerformed);
}
