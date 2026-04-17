using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player input for a specific device.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public int Index => transform.GetSiblingIndex();
    public int Id => Index + 1;
    public bool IsInputEnabled
    {
        get
        {
            if (PlayerManager.Instance.IsSingleDeviceMode)
            {
                return _sdActionMap != null && _sdActionMap.enabled;
            }
            return PlayerInput.inputIsActive;
        }
    }
    public InputActionAsset ActionsAsset
    {
        get
        {
            if (PlayerManager.Instance.IsSingleDeviceMode)
            {
                return PlayerManager.Instance.EventSystemActionsAsset;
            }
            return PlayerInput.actions;
        }
    }
    public InputDevice Device
    {
        get
        {
            if (PlayerManager.Instance.IsSingleDeviceMode)
            {
                foreach (var d in InputSystem.devices)
                {
                    if (d is Keyboard)
                    {
                        return d;
                    }
                }
            }
            _device ??= PlayerInput.devices[0];
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
    private InputActionMap _sdActionMap;

    private void Start()
    {
        if (PlayerManager.Instance.IsSingleDeviceMode)
        {
            string actionMapName = $"SDPlayer{Id}";
            // In SD mode, all players share the EventSystem's global action asset
            // — each controller subscribes to its own SDPlayerN map on that shared asset.
            _sdActionMap = PlayerManager.Instance.EventSystemActionsAsset
                .actionMaps.First((s) => s.name == actionMapName);

            foreach (var action in _sdActionMap.actions)
            {
                switch (action.name)
                {
                    case "Move":
                        action.performed += InputMove;
                        break;
                    case "Interact":
                        action.performed += InputInteract;
                        break;
                    case "PromptSouth":
                        action.performed += InputPromptSouth;
                        break;
                    case "PromptWest":
                        action.performed += InputPromptWest;
                        break;
                    case "PromptEast":
                        action.performed += InputPromptEast;
                        break;
                    case "PromptNorth":
                        action.performed += InputPromptNorth;
                        break;
                    case "Cancel":
                        action.performed += InputCancel;
                        break;
                }
            }
        }

        var currentOwner = PlayerManager.Instance.CurrentOwner;
        if (currentOwner != null && currentOwner != this)
        {
            DisableInput();
        }
    }

    private void TryPerform(InputAction.CallbackContext context, Action action)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            action?.Invoke();
        }
    }

    public void EnableInput()
    {
        if (PlayerManager.Instance.IsSingleDeviceMode)
        {
            _sdActionMap?.Enable();
        }
        else
        {
            PlayerInput.ActivateInput();
            InputSystem.EnableDevice(Device);
        }
        CancelPerformed += PopMenu;
    }

    public void DisableInput()
    {
        if (PlayerManager.Instance.IsSingleDeviceMode)
        {
            _sdActionMap?.Disable();
        }
        else
        {
            PlayerInput.DeactivateInput();
            InputSystem.DisableDevice(Device);
        }
        CancelPerformed -= PopMenu;
    }

    private void PopMenu()
    {
        MenuNavigator.Pop();
    }

    private void OnDestroy()
    {
        if (PlayerManager.Instance == null)
        {
            return;
        }
        if (PlayerManager.Instance.IsSingleDeviceMode)
        {
            // SDPlayerN map lives on the shared global asset — unsubscribe our
            // handlers so they don't accumulate across joins/leaves.
            if (_sdActionMap != null)
            {
                foreach (var action in _sdActionMap.actions)
                {
                    switch (action.name)
                    {
                        case "Move": action.performed -= InputMove; break;
                        case "Interact": action.performed -= InputInteract; break;
                        case "PromptSouth": action.performed -= InputPromptSouth; break;
                        case "PromptWest": action.performed -= InputPromptWest; break;
                        case "PromptEast": action.performed -= InputPromptEast; break;
                        case "PromptNorth": action.performed -= InputPromptNorth; break;
                        case "Cancel": action.performed -= InputCancel; break;
                    }
                }
            }
            return;
        }
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
