using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardPlayerTurnCard : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _playerNumberText;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private Image _currentTurnIcon;

    private BoardPlayerData _playerData;

    public void Setup(BoardPlayerData playerData)
    {
        _playerData = playerData;
        var profile = PlayerManager.Instance.GetPlayerProfile(_playerData.Index);
        _icon.color = profile.Color;
        _playerNumberText.SetText($"PLAYER {_playerData.Index+1}");
        _playerData.OnCoinCountChanged += OnCoinsUpdate;
        BoardManager.Instance.OnNextTurn += OnNextTurn;
        _currentTurnIcon.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _playerData.OnCoinCountChanged -= OnCoinsUpdate;
        if (BoardManager.Instance != null)
        {
            BoardManager.Instance.OnNextTurn -= OnNextTurn;
        }
    }

    private void OnCoinsUpdate()
    {
        _coinsText.SetText(_playerData.CoinCount.ToString());
    }

    private void OnNextTurn()
    {
        if (GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index) == _playerData)
        {
            _currentTurnIcon.gameObject.SetActive(true);
        }
        else
        {
            _currentTurnIcon.gameObject.SetActive(false);
        }
    }
}
