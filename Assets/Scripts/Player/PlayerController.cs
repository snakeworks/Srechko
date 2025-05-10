using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
            //Debug.Log($"Action pressed from: {context.control.device}");
        }
    }

    public void EnableInput()
    {
        _playerInput.ActivateInput();
        InputSystem.EnableDevice(Device); 
    }

    public void DisableInput()
    {
        _playerInput.DeactivateInput();
        InputSystem.DisableDevice(Device); 
    }

    public void InputInteract(InputAction.CallbackContext context) => TryPerform(context, InteractPerformed);
    public void InputOpenPauseMenu(InputAction.CallbackContext context) => TryPerform(context, OpenPauseMenuPerformed);
    public void InputCancel(InputAction.CallbackContext context) => TryPerform(context, CancelPerformed);
}
