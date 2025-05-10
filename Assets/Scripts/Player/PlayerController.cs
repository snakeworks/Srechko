using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool IsInputEnabled => _playerInput.inputIsActive;
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
        InputSystem.EnableDevice(_playerInput.devices[0]); // TODO: Could cause problem in the future???
    }

    public void DisableInput()
    {
        _playerInput.DeactivateInput();
        InputSystem.DisableDevice(_playerInput.devices[0]); // TODO: Could cause problem in the future???
    }

    public void InputInteract(InputAction.CallbackContext context) => TryPerform(context, InteractPerformed);
    public void InputOpenPauseMenu(InputAction.CallbackContext context) => TryPerform(context, OpenPauseMenuPerformed);
    public void InputCancel(InputAction.CallbackContext context) => TryPerform(context, CancelPerformed);
}
