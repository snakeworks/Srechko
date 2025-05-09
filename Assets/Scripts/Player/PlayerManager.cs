using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public PlayerInput MainPlayerInput => GetPlayerInput(0);
    
    private InputActions _input;
    private PlayerInputManager _playerInputManager;

    private readonly List<PlayerInput> _playerInputs = new();

    private void Awake()
    {
        _input = new();
        _input.Enable();

        _playerInputManager = GetComponent<PlayerInputManager>();

        _playerInputManager.onPlayerJoined += OnPlayerJoined;
        _playerInputManager.onPlayerLeft += OnPlayerLeft;
    }

    public PlayerInput GetPlayerInput(int index)
    {
        if (!_playerInputs.InRange(index))
        {
            return null;
        }
        return _playerInputs[index];
    }

    private void OnPlayerJoined(PlayerInput player)
    {
        Debug.Log($"Joined: {player}");
        player.transform.SetParent(transform);
        player.gameObject.name = $"PlayerInput{_playerInputManager.playerCount}";
        _playerInputs.Add(player);
    }

    private void OnPlayerLeft(PlayerInput player)
    {
        Debug.Log($"Left: {player}");
        _playerInputs.Remove(player);
    }
}
