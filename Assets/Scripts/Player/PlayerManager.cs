using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

/// <summary>
/// Handles multiple <c>PlayerController</c> scripts.
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private Menu _pauseMenu;
    [SerializeField] private PlayerProfile[] _playerProfiles;

    public bool IsInputEnabled { get; private set; } = true;
    public PlayerController MainPlayerController => GetPlayerController(0);
    public bool HasMinimumPlayerCount => _controllers.Count > 1;
    public PlayerController CurrentOwner { get; private set; }
    public event Action<PlayerController> OnNewOwner;
    public event Action<PlayerController> OnNewMainPlayerController;
    public event Action<PlayerController> OnPlayerJoin;
    public event Action<PlayerController> OnPlayerLeave;
    
    private PlayerInputManager _playerInputManager;
    private readonly List<PlayerController> _controllers = new();
    private InputActionAsset _eventSystemDefaultActionsAsset;
    private InputSystemUIInputModule _uiInputModule;

    protected override void Init()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();

        _playerInputManager.onPlayerJoined += OnPlayerJoined;
        _playerInputManager.onPlayerLeft += OnPlayerLeft;

        // Gotta GetComponent the UI input module because this script initializes
        // faster than the EventSystem, even in the start method. Bit stupid, but works.
        _uiInputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
        _eventSystemDefaultActionsAsset = _uiInputModule.actionsAsset;
    }

    /// <summary>
    /// Trys to find a player controller with the <c>index</c>.
    /// </summary>
    /// <returns>The <c>PlayerController</c>, if it was found, otherwise null.</returns>
    public PlayerController GetPlayerController(int index)
    {
        if (!_controllers.InRange(index))
        {
            return null;
        }
        return _controllers[index];
    }

    public PlayerProfile GetPlayerProfile(int index)
    {
        if (!_playerProfiles.InRange(index))
        {
            return null;
        }
        return _playerProfiles[index];
    }

    /// <summary>
    /// Gives ownership of the input system to this <c>controller</c>
    /// and revokes ownership for all other player controllers.
    /// An Owner is a device that currently has control of the game.
    /// There can only be one Owner, however if there is no Owner (null), 
    /// then every input device has ownership of the input system.
    /// Ownership can be changed while input is disabled, but the owner will not gain
    /// control of input, only the owner will change. You must re-enable
    /// the input first with <c>EnableInput</c>. This is only necessary if
    /// you previously called <c>DisableInput</c> and didn't re-enable it.
    /// </summary>
    /// <param name="controller">The controller to give the ownership to.</param>
    public void GiveOwnershipTo(PlayerController controller)
    {
        if (controller == null)
        {
            return;
        }
        foreach (var con in _controllers)
        {
            con.DisableInput();
        }
        CurrentOwner = controller;
        if (IsInputEnabled)
        {
            controller.EnableInput();
        }
        _uiInputModule.actionsAsset = controller.ActionsAsset;
        OnNewOwner?.Invoke(CurrentOwner);
    }

    /// <summary>
    /// Gives ownership of the input system to all player controllers.
    /// Ownership can be changed while the input is disabled, but the
    /// new owners will not gain control of the input. You must re-enable
    /// the input first with <c>EnableInput</c>. This is only necessary if
    /// you previously called <c>DisableInput</c> and didn't re-enable it.
    /// </summary>
    public void GiveOwnershipToAll()
    {
        CurrentOwner = null;
        if (IsInputEnabled)
        {
            foreach (var controller in _controllers)
            {
                controller.EnableInput();
            }
        }
        _uiInputModule.actionsAsset = _eventSystemDefaultActionsAsset;
        OnNewOwner?.Invoke(null);
    }

    /// <summary>
    /// Enables the ability to send input events to the game.
    /// This has no effect on ownership. The owner of the input will stay the same as before.
    /// </summary>
    public void EnableInput()
    {
        if (IsInputEnabled)
        {
            return;
        }
        IsInputEnabled = true;
        if (CurrentOwner != null)
        {
            GiveOwnershipTo(CurrentOwner);
        }
        else
        {
            GiveOwnershipToAll();
        }
    }

    /// <summary>
    /// Disables the ability to send input events to the game.
    /// This has no effect on ownership. The owner of the input will stay the same as before.
    /// </summary>
    public void DisableInput()
    {
        if (!IsInputEnabled)
        {
            return;
        }
        IsInputEnabled = false;
        foreach (var con in _controllers)
        {
            con.DisableInput();
        }
    }

    /// <summary>
    /// Enables the ability for other devices to join into the game
    /// as a new <c>PlayerController</c>.
    /// </summary>
    public void EnableJoining()
    {
        _playerInputManager.EnableJoining();
    }

    /// <summary>
    /// Disables the ability for other devices to join into the game.
    /// </summary>
    public void DisableJoining()
    {
        _playerInputManager.DisableJoining();
    }

    private void OnPlayerJoined(PlayerInput player)
    {
        Debug.Log($"Joined: {player}");
        player.transform.SetParent(transform);
        player.gameObject.name = $"PlayerController{_playerInputManager.playerCount}";

        var controller = player.GetComponent<PlayerController>();
        controller.OpenPauseMenuPerformed += () => OpenPauseMenu(controller);
        _controllers.Add(controller);

        if (CurrentOwner != null)
        {
            controller.DisableInput();
        }

        if (_controllers.Count == 1)
        {
            OnNewMainPlayerController?.Invoke(controller);
        }

        UpdateControllerNames();
        OnPlayerJoin?.Invoke(controller);
    }

    private void OnPlayerLeft(PlayerInput player)
    {
        Debug.Log($"Left: {player}");
        var controller = player.GetComponent<PlayerController>();
        _controllers.Remove(controller);

        if (_controllers.Count > 0)
        {
            if (controller == CurrentOwner)
            {
                GiveOwnershipTo(MainPlayerController);
            }
            OnNewMainPlayerController?.Invoke(MainPlayerController);
        }
        UpdateControllerNames();
        OnPlayerLeave?.Invoke(controller);
    }

    private void OpenPauseMenu(PlayerController controller)
    {
        if (!MenuNavigator.IsStackEmpty)
        {
            return;
        }

        GiveOwnershipTo(controller);

        MenuNavigator.OnEmptyStack += OnEmptyStack;
        
        void OnEmptyStack()
        {
            MenuNavigator.OnEmptyStack -= OnEmptyStack;
            GiveOwnershipToAll();
        }

        MenuNavigator.Push(_pauseMenu);
    }

    private void UpdateControllerNames()
    {
        for (int i = 0; i < _controllers.Count; i++)
        {
            _controllers[i].name = $"PlayerController{i+1}";
        }
    }
}
