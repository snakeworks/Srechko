using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbySlot : MonoBehaviour
{
    [SerializeField] private GameObject _emptyPlayerOverlay;
    [SerializeField] private GameObject _joinedPlayerOverlay;
    [SerializeField] private Image _playerIcon;
    [SerializeField] private Image _playerNumberBackground;
    [SerializeField] private TextMeshProUGUI _playerNumberText;
    [SerializeField] private TextMeshProUGUI _deviceText;
    [SerializeField] private Image _sdControlsImage;
    [SerializeField] private Sprite[] _sdControlsSprites;

    public int SlotIndex => transform.GetSiblingIndex();
    
    private void Awake()
    {
        _playerNumberText.SetText($"PLAYER {SlotIndex+1}");
        PlayerManager.Instance.OnPlayerJoin += OnPlayerJoin;
        PlayerManager.Instance.OnPlayerLeave += OnPlayerLeave;
        UpdateSlot();
    }

    private void OnDestroy()
    {
        if (PlayerManager.Instance == null)
        {
            return;
        }
        PlayerManager.Instance.OnPlayerJoin -= OnPlayerJoin;
        PlayerManager.Instance.OnPlayerLeave -= OnPlayerLeave;
    }

    private void OnPlayerJoin(PlayerController controller)
    {   
        UpdateSlot();
    }

    private void OnPlayerLeave(PlayerController controller)
    {
        UpdateSlot();
    }

    private void UpdateSlot()
    {
        if (PlayerManager.Instance.GetPlayerController(SlotIndex) == null)
        {
            _joinedPlayerOverlay.SetActive(false);
            _emptyPlayerOverlay.SetActive(true);
        }
        else
        {
            var profile = PlayerManager.Instance.GetPlayerProfile(SlotIndex);
            _playerIcon.color = profile.Color;
            _playerNumberBackground.color = profile.Color;

            if (PlayerManager.Instance.IsSingleDeviceMode)
            {
                _sdControlsImage.sprite = _sdControlsSprites[SlotIndex];
                _deviceText.SetText("");
            }
            else
            {
                var controller = PlayerManager.Instance.GetPlayerController(SlotIndex);
                _deviceText.SetText($"Device Id: {controller.Device.deviceId}\n{controller.Device.description}\n");
            }

            _joinedPlayerOverlay.SetActive(true);
            _emptyPlayerOverlay.SetActive(false);
        }
    }
}
