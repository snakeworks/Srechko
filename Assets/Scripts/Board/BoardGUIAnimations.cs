using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BoardGUIAnimations : StaticInstance<BoardGUIAnimations>
{
    [SerializeField] private TextMeshProUGUI _playerTurnText;
    [SerializeField] private CanvasGroup _playerTurnCanvasGroup;

    protected override void Awake()
    {
        base.Awake();
        _playerTurnCanvasGroup.gameObject.SetActive(false);
    }

    public async Task AnimatePlayerTurn()
    {
        Sequence sequence = DOTween.Sequence();

        _playerTurnCanvasGroup.gameObject.SetActive(true);
        _playerTurnCanvasGroup.alpha = 0.0f;
        var rectTransform = _playerTurnCanvasGroup.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new(rectTransform.anchoredPosition.x, -50.0f);

        var currentPlayer = BoardManager.Instance.CurrentPlayer;
        _playerTurnText.SetText($"PLAYER {currentPlayer.Index+1} TURN");

        sequence.Insert(0.0f, rectTransform.DOAnchorPosY(0.0f, 0.15f));
        sequence.Insert(0.0f, _playerTurnCanvasGroup.DOFade(1.0f, 0.15f));
        sequence.Insert(1.5f, _playerTurnCanvasGroup.DOFade(0.0f, 0.15f));

        await sequence.AsyncWaitForCompletion();

        _playerTurnCanvasGroup.gameObject.SetActive(false);
    }
}
