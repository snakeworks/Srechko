using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private Menu _pauseMenu;

    public PlayerController MainPlayerController => GetPlayerController(0);
    public PlayerController CurrentOwner { get; private set; }
    public event Action<PlayerController> OnNewOwner;
    public event Action<PlayerController> OnNewMainPlayerController;
    public event Action<PlayerController> OnPlayerJoin;
    public event Action<PlayerController> OnPlayerLeave;
    
    private PlayerInputManager _playerInputManager;
    private readonly List<PlayerController> _controllers = new();
    private readonly Dictionary<PlayerController, InputDevice> _disabledInputDevices = new();

    protected override void Init()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();

        _playerInputManager.onPlayerJoined += OnPlayerJoined;
        _playerInputManager.onPlayerLeft += OnPlayerLeft;
    }

    public PlayerController GetPlayerController(int index)
    {
        if (!_controllers.InRange(index))
        {
            return null;
        }
        return _controllers[index];
    }

    public int GetIndexOfPlayerController(PlayerController controller)
    {
        for (int i = 0; i < _controllers.Count; i++)
        {
            if (controller == _controllers[i])
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Gives ownership of the input system to this <c>controller</c>
    /// and revokes ownership for all other player controllers.
    /// </summary>
    /// <param name="controller">The controller to give the ownership to.</param>
    public void GiveOwnershipTo(PlayerController controller)
    {
        if (controller == null)
        {
            return;
        }
        DisableAllInput();
        CurrentOwner = controller;
        controller.EnableInput(_disabledInputDevices);
        OnNewOwner?.Invoke(CurrentOwner);
    }

    /// <summary>
    /// Gives ownership of the input system to all player controllers.
    /// </summary>
    public void GiveOwnershipToAll()
    {
        CurrentOwner = null;
        foreach (var controller in _controllers)
        {
            controller.EnableInput(_disabledInputDevices);
        }
        OnNewOwner?.Invoke(null);
    }

    /// <summary>
    /// Enables the ability to send input events to the game.
    /// </summary>
    public void EnableInput()
    {
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
    /// </summary>
    public void DisableAllInput()
    {
        foreach (var con in _controllers)
        {
            con.DisableInput(_disabledInputDevices);
        }
    }

    public void EnableJoining()
    {
        _playerInputManager.EnableJoining();
    }

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
            controller.DisableInput(_disabledInputDevices);
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

        if (_disabledInputDevices.TryGetValue(controller, out var device))
        {
            InputSystem.EnableDevice(device);
            _disabledInputDevices.Remove(controller);
        }

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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        foreach (var entry in _disabledInputDevices)
        {
            InputSystem.EnableDevice(entry.Value);
        }
    }
}
