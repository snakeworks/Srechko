using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private Menu _pauseMenu;

    public PlayerController MainPlayerController => GetPlayerController(0);    
    
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

    private void OnPlayerJoined(PlayerInput player)
    {
        Debug.Log($"Joined: {player}");
        player.transform.SetParent(transform);
        player.gameObject.name = $"PlayerController{_playerInputManager.playerCount}";

        var controller = player.GetComponent<PlayerController>();
        controller.OpenPauseMenuPerformed += () => OpenPauseMenu(controller);
        _controllers.Add(controller);
    }

    private void OnPlayerLeft(PlayerInput player)
    {
        Debug.Log($"Left: {player}");
        var controller = player.GetComponent<PlayerController>();
        _controllers.Remove(controller);
    }

    private void OpenPauseMenu(PlayerController controller)
    {
        if (!MenuNavigator.IsStackEmpty)
        {
            return;
        }

        List<bool> previousControllerEnableStates = new();
        foreach (var con in _controllers)
        {
            previousControllerEnableStates.Add(con.IsInputEnabled);
        }
        EnableOnlyController(GetIndexOfPlayerController(controller));
        controller.CancelPerformed += OnCancel;
        MenuNavigator.OnEmptyStack += OnEmptyStack;

        void OnCancel()
        {
            MenuNavigator.Pop();
        }
        
        void OnEmptyStack()
        {
            controller.CancelPerformed -= OnCancel;
            MenuNavigator.OnEmptyStack -= OnEmptyStack;
            for (int i = 0; i < previousControllerEnableStates.Count; i++)
            {
                if (previousControllerEnableStates[i])
                {
                    _controllers[i].EnableInput();
                }
                else
                {
                    _controllers[i].DisableInput();
                }
            }
        }

        MenuNavigator.Push(_pauseMenu);
    }

    private void EnableOnlyController(int index)
    {
        if (GetPlayerController(index))
        {
            DisableAllControllers();
            GetPlayerController(index).EnableInput();
        }
    }

    private void DisableAllControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.DisableInput();
        }
    }
}
