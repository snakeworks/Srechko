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
    [SerializeField] private DeveloperMenu _devMenu;
    [SerializeField] private PlayerProfile[] _playerProfiles;

    public bool IsSingleDeviceMode { get; private set; } = true;
    public bool IsInputEnabled { get; private set; } = true;
    public PlayerController MainPlayerController => GetPlayerController(0);
    public bool HasMinimumPlayerCount => _controllers.Count > 1;
    public int ControllerCount => _controllers.Count;
    public PlayerController CurrentOwner { get; private set; }
    public List<PlayerResult> MatchResults { get; private set; }
    public InputActionAsset EventSystemActionsAsset => _eventSystemDefaultActionsAsset;
    public event Action<PlayerController> OnNewOwner;
    public event Action<PlayerController> OnNewMainPlayerController;
    public event Action<PlayerController> OnPlayerJoin;
    public event Action<PlayerController> OnPlayerLeave;
    public event Action<PlayerController> OnAnyPlayerInteractPerformed;
    public event Action<PlayerController> OnAnyPlayerPromptSouthPerformed;
    public event Action<PlayerController> OnAnyPlayerPromptEastPerformed;
    public event Action<PlayerController> OnAnyPlayerPromptWestPerformed;
    public event Action<PlayerController> OnAnyPlayerPromptNorthPerformed;
    
    private PlayerInputManager _playerInputManager;
    private readonly List<PlayerController> _controllers = new();
    private InputActionAsset _eventSystemDefaultActionsAsset;
    private InputSystemUIInputModule _uiInputModule;
    private InputActionMap _uiMap;
    private bool _isJoiningEnabled = true;

    // Non-keyboard groups in the UI map that should stay live regardless of SD owner.
    private const string SharedUiGroups = "Keyboard&Mouse;Gamepad;Joystick;Touch;XR";

    protected override void Init()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();

        // Gotta GetComponent the UI input module because this script initializes
        // faster than the EventSystem, even in the start method. Bit stupid, but works.
        _uiInputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
        _eventSystemDefaultActionsAsset = _uiInputModule.actionsAsset;
        _uiMap = _eventSystemDefaultActionsAsset.FindActionMap("UI");

        // Single-device join: each SDPlayerN map's Interact action triggers JoinPlayer(N - 1).
        // Subscribed on the EventSystem's global action asset so it fires regardless of ownership.
        foreach (var map in _eventSystemDefaultActionsAsset.actionMaps)
        {
            if (!map.name.StartsWith("SDPlayer")) continue;

            var interact = map.FindAction("Interact");
            if (interact == null) continue;

            int playerIndex = int.Parse(map.name[^1].ToString()) - 1;
            interact.performed += _ => JoinPlayer(playerIndex);
        }
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
        if (IsSingleDeviceMode)
        {
            // Mask the shared UI map so only the owner's keyboard bindings
            // (tagged "SDPlayerN") fire Navigate/Submit. Non-keyboard groups
            // stay on so mouse/gamepad UI still works.
            if (_uiMap != null)
            {
                _uiMap.bindingMask = new InputBinding
                {
                    groups = $"SDPlayer{controller.Id};{SharedUiGroups}"
                };
            }
        }
        else
        {
            _uiInputModule.actionsAsset = controller.ActionsAsset;
        }
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
        if (IsSingleDeviceMode)
        {
            if (_uiMap != null)
            {
                _uiMap.bindingMask = null;
            }
        }
        else
        {
            _uiInputModule.actionsAsset = _eventSystemDefaultActionsAsset;
        }
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
        _isJoiningEnabled = true;
        _playerInputManager.EnableJoining();
    }

    /// <summary>
    /// Disables the ability for other devices to join into the game.
    /// </summary>
    public void DisableJoining()
    {
        _isJoiningEnabled = false;
        _playerInputManager.DisableJoining();
    }

    public void SetResults(List<PlayerResult> results)
    {
        MatchResults = results;
    }

    private void JoinPlayer(int playerIndex)
    {
        if (!_isJoiningEnabled) return;
        if (_controllers.Count >= _playerInputManager.maxPlayerCount) return;

        // Players must join in slot order (Player 1 before Player 2, etc.).
        if (playerIndex != _controllers.Count) return;

        PlayerController controller;

        if (IsSingleDeviceMode)
        {
            // Bypass PlayerInputManager.JoinPlayer — all SD players share the same
            // keyboard, so per-player device pairing would fail. Instantiate the
            // prefab inactive, disable its PlayerInput (we route through the
            // EventSystem's global action asset instead), then activate it.
            var prefab = _playerInputManager.playerPrefab;
            bool wasPrefabActive = prefab.activeSelf;
            prefab.SetActive(false);
            var go = Instantiate(prefab, transform);
            prefab.SetActive(wasPrefabActive);

            var pi = go.GetComponent<PlayerInput>();
            if (pi != null) pi.enabled = false;

            go.SetActive(true);
            controller = go.GetComponent<PlayerController>();
        }
        else
        {
            PlayerInput player = _playerInputManager.JoinPlayer(playerIndex);
            if (player == null) return;
            player.transform.SetParent(transform);
            controller = player.GetComponent<PlayerController>();
        }

        AudioManager.Instance.Play(SoundName.DiceRollFinished);

        controller.InteractPerformed += () => OnAnyPlayerInteractPerformed?.Invoke(controller);
        controller.PromptSouthPerformed += () => OnAnyPlayerPromptSouthPerformed?.Invoke(controller);
        controller.PromptEastPerformed += () => OnAnyPlayerPromptEastPerformed?.Invoke(controller);
        controller.PromptWestPerformed += () => OnAnyPlayerPromptWestPerformed?.Invoke(controller);
        controller.PromptNorthPerformed += () => OnAnyPlayerPromptNorthPerformed?.Invoke(controller);
        controller.OpenDevMenuPerformed += TryOpenDevMenu;

        _controllers.Add(controller);

        if (_controllers.Count == 1)
        {
            OnNewMainPlayerController?.Invoke(controller);
        }

        UpdateControllerNames();
        OnPlayerJoin?.Invoke(controller);
    }

    // Removes a joined controller and reassigns ownership to Player 1 if the leaver was owner.
    // Not currently reachable in single-device mode — no key is bound to trigger it.
    public void LeavePlayer(int index)
    {
        var controller = GetPlayerController(index);
        if (controller == null) return;

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

        Destroy(controller.gameObject);
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

    private static int _devMenuOpenCount = 0;
    private static DateTime _lastDevMenuAttempt = DateTime.Now;

    private void TryOpenDevMenu()
    {
        if (_devMenu.IsOnStack) return;

        var time = DateTime.Now.Subtract(_lastDevMenuAttempt);
        _lastDevMenuAttempt = DateTime.Now;
        if (time > TimeSpan.FromSeconds(0.25f))
        {
            _devMenuOpenCount = 1;
            return;
        }
        _devMenuOpenCount++;

        if (_devMenuOpenCount >= 5)
        {
            _devMenu.TryPush();
            _devMenuOpenCount = 0;
        }
    }

    private void UpdateControllerNames()
    {
        for (int i = 0; i < _controllers.Count; i++)
        {
            _controllers[i].name = $"PlayerController{i + 1}";
        }
    }
}
