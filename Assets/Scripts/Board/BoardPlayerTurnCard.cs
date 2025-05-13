using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardPlayerTurnCard : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _playerNumberText;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private RectTransform _visualsRect;

    private BoardPlayerData _playerData;
    private bool _hadLastTurn = false;

    public void Setup(BoardPlayerData playerData)
    {
        _playerData = playerData;
        var profile = PlayerManager.Instance.GetPlayerProfile(_playerData.Index);
        _icon.color = profile.Color;
        _playerNumberText.SetText($"PLAYER {_playerData.Index+1}");
        _playerData.OnCoinCountChanged += OnCoinsUpdate;
        BoardManager.Instance.OnNextTurn += OnNextTurn;
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
            _hadLastTurn = true;
            _visualsRect.DOAnchorPos3DY(25.0f, 0.15f);
        }
        else
        {
            if (_hadLastTurn)
            {
                _visualsRect.DOAnchorPos3DY(0.0f, 0.15f);
            }
            _hadLastTurn = false;
        }
    }
}
