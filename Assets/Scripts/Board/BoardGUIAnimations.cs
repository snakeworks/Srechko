using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BoardGUIAnimations : StaticInstance<BoardGUIAnimations>
{
    [SerializeField] private TextMeshProUGUI _popupText;
    [SerializeField] private CanvasGroup _popupCanvasGroup;
    [SerializeField] private RectTransform _roundPanelRect;
    [SerializeField] private TextMeshProUGUI _roundCountText;
    [SerializeField] private TextMeshProUGUI _roundMaxCountText;

    private float _roundPanelRectFinalX;

    protected override void Awake()
    {
        base.Awake();
        _popupCanvasGroup.gameObject.SetActive(false);
        _roundPanelRectFinalX = _roundPanelRect.anchoredPosition.x;
        _roundPanelRect.DOAnchorPosX(-170.0f, 0.0f);
    }

    private void Start()
    {
        _roundMaxCountText.SetText(BoardManager.MaxRoundCount.ToString("D2"));
        BoardManager.Instance.OnTurnOrderSet += () =>
        {
            _roundPanelRect.DOAnchorPosX(_roundPanelRectFinalX, 0.25f);
        };
        BoardManager.Instance.OnGameEnd += () =>
        {
            _roundPanelRect.DOAnchorPosX(-170.0f, 0.25f);
        };
        BoardManager.Instance.OnNextRound += () =>
        {
            _roundCountText.SetText(
                Mathf.Clamp(BoardManager.Instance.CurrentRound, 1, BoardManager.MaxRoundCount).ToString("D2")
            );
        };
    }

    public async Task PlayPopupAnimation(string text)
    {
        Sequence sequence = DOTween.Sequence();

        _popupCanvasGroup.gameObject.SetActive(true);
        _popupCanvasGroup.alpha = 0.0f;
        var rectTransform = _popupCanvasGroup.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new(rectTransform.anchoredPosition.x, -50.0f);

        _popupText.SetText(text);

        sequence.Insert(0.0f, rectTransform.DOAnchorPosY(0.0f, 0.15f));
        sequence.Insert(0.0f, _popupCanvasGroup.DOFade(1.0f, 0.15f));
        sequence.Insert(1.5f, _popupCanvasGroup.DOFade(0.0f, 0.15f));

        await sequence.AsyncWaitForCompletion();

        _popupCanvasGroup.gameObject.SetActive(false);
    }

    public async Task PlayPlayerTurnAnimation()
    {
        var currentPlayer = BoardManager.Instance.CurrentPlayer;
        if (currentPlayer.SkipNextTurn) AudioManager.Instance.Play(SoundName.SkipTurn);
        else AudioManager.Instance.Play(SoundName.NextTurn);
        await PlayPopupAnimation(!currentPlayer.SkipNextTurn
                ? $"PLAYER {currentPlayer.Index + 1} TURN"
                : $"SKIPPING PLAYER {currentPlayer.Index + 1}");
    }
}
