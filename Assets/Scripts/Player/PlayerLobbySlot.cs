using TMPro;
using UnityEngine;

public class PlayerLobbySlot : MonoBehaviour
{
    [SerializeField] private GameObject _emptyPlayerOverlay;
    [SerializeField] private GameObject _joinedPlayerOverlay;
    [SerializeField] private TextMeshProUGUI _playerNumberText;

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
            _joinedPlayerOverlay.SetActive(true);
            _emptyPlayerOverlay.SetActive(false);   
        }
    }
}
