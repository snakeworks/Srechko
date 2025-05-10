using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private Menu _pauseMenu;

    public PlayerController MainPlayerController => GetPlayerController(0);
    public PlayerController CurrentOwner { get; private set; }
    public event Action<PlayerController> OnNewOwner;
    public event Action<PlayerController> OnNewMainPlayerController;
    
    private PlayerInputManager _playerInputManager;
    private readonly List<PlayerController> _controllers = new();

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

    public void GiveOwnershipTo(PlayerController controller)
    {
        if (controller == null)
        {
            return;
        }
        foreach (var con in _controllers)
        {
            con.RevokeOwnership();
        }
        CurrentOwner = controller;
        controller.GiveOwnership();
        OnNewOwner?.Invoke(CurrentOwner);
    }

    public void GiveOwnershipToAll()
    {
        if (CurrentOwner != null)
        {
            CurrentOwner.RevokeOwnership();
        }
        CurrentOwner = null;
        foreach (var controller in _controllers)
        {
            controller.GiveOwnership();
        }
        OnNewOwner?.Invoke(null);
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
            controller.RevokeOwnership();
        }

        if (_controllers.Count == 1)
        {
            OnNewMainPlayerController?.Invoke(controller);
        }
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

        MenuNavigator.Push(_pauseMenu, controller);
    }
}
