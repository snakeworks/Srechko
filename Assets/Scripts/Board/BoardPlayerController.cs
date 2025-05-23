using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BoardPlayerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _visuals;
    [SerializeField] private Transform _cameraView;
    [SerializeField] private CanvasGroup _diceCanvasGroup;
    [SerializeField] private TextMeshProUGUI _diceNumberText;
    [SerializeField] private CanvasGroup _coinsCanvasGroup;
    [SerializeField] private TextMeshProUGUI _coinsText;

    public int StandingOnBoardSpaceId { get; private set; } = -1;
    public Transform CameraView => _cameraView;
    public int Index => transform.GetSiblingIndex();
    public int LastRolledDiceNumber { get; private set; }

    private int _moveCountModifier = 0;
    private WaitForSeconds _numberGenerationDelay = new(0.02f);
    private Coroutine _generateRandomNumberCoroutine;

    private void Awake()
    {
        var profile = PlayerManager.Instance.GetPlayerProfile(Index);
        _visuals.material.SetColor("SpriteColor", profile.Color);
        
        _diceCanvasGroup.gameObject.SetActive(false);
        _diceCanvasGroup.alpha = 0.0f;
        _coinsCanvasGroup.gameObject.SetActive(false);
        _coinsCanvasGroup.alpha = 0.0f;
    }

    public void ShowDiceRolling()
    {
        _diceCanvasGroup.gameObject.SetActive(true);
        _diceCanvasGroup.alpha = 0.0f;

        _diceCanvasGroup.DOFade(1.0f, 0.15f);
        _diceNumberText.SetText(
            (Random.Range(BoardManager.MinDiceNumber, BoardManager.MaxDiceNumber) + _moveCountModifier).ToString()
        );
        _generateRandomNumberCoroutine = StartCoroutine(GenerateRandomNumber());
        IEnumerator GenerateRandomNumber()
        {
            while (true)
            {
                yield return _numberGenerationDelay;
                _diceNumberText.SetText(
                    (Random.Range(BoardManager.MinDiceNumber, BoardManager.MaxDiceNumber) + _moveCountModifier).ToString()
                );
            }
        }
    }

    public void FinishRollingDice(int numberRolled)
    {
        LastRolledDiceNumber = numberRolled + _moveCountModifier;
        _moveCountModifier = 0;
        StopCoroutine(_generateRandomNumberCoroutine);
        SetDiceNumberText(LastRolledDiceNumber);
    }

    public void SetDiceNumberText(int number)
    {
        _diceNumberText.SetText(number.ToString());
    }

    public void HideDice()
    {
        _diceCanvasGroup.DOFade(0.0f, 0.15f).OnComplete(() => _diceCanvasGroup.gameObject.SetActive(false));
    }

    public void MoveToSpace(BoardSpace space, System.Action onComplete)
    {
        StandingOnBoardSpaceId = space.Id;
        _visuals.transform.DOLocalMoveY(0.4f, 0.1f).SetEase(Ease.OutQuad).OnComplete(() => {
            _visuals.transform.DOLocalMoveY(0.0f, 0.4f).SetEase(Ease.OutBounce).OnComplete(() => onComplete());
        });
        transform.DOMoveX(space.transform.position.x, 0.3f);
        transform.DOMoveZ(space.transform.position.z, 0.3f);
    }

    public void SetMoveCountModifier(int modifier)
    {
        _moveCountModifier = modifier;
    }

    public void PlayCoinsGet(int amount)
    {
        _coinsText.SetText($"+{amount} <sprite index=0>");
        _coinsCanvasGroup.gameObject.SetActive(true);

        _coinsCanvasGroup.alpha = 0.0f;
        var rectTransform = _coinsCanvasGroup.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new(
            rectTransform.anchoredPosition.x,
            -230.0f
        );

        _coinsCanvasGroup.DOFade(1.0f, 0.1f);
        rectTransform.DOAnchorPosY(0.0f, 0.1f);
        _coinsCanvasGroup.DOFade(0.0f, 0.1f).SetDelay(1.0f).OnComplete(() => _coinsCanvasGroup.gameObject.SetActive(false));
    }
}
