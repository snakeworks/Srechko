using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BoardPlayerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _visuals;
    [SerializeField] private Transform _cameraView;
    [SerializeField] private CanvasGroup _diceCanvasGroup;
    [SerializeField] private CanvasGroup _coinsCanvasGroup;
    [SerializeField] private TextMeshProUGUI[] _diceTexts = new TextMeshProUGUI[_diceCountMax];
    [SerializeField] private TextMeshProUGUI _finalDiceNumberText;
    [SerializeField] private TextMeshProUGUI _coinsText;

    public int StandingOnBoardSpaceId { get; private set; } = -1;
    public Transform CameraView => _cameraView;
    public int Index => transform.GetSiblingIndex();
    public int LastRolledDiceNumber { get; private set; } = -1;

    private int _moveCountModifier = 0;
    private int _diceCount = _diceCountMin;
    private int _currentRollingDiceIndex = 0;
    private readonly WaitForSeconds _numberGenerationDelay = new(0.02f);
    private Coroutine _generateRandomNumberCoroutine;
    private const int _diceCountMin = 1;
    private const int _diceCountMax = 3;

    private void Awake()
    {
        var profile = PlayerManager.Instance.GetPlayerProfile(Index);
        _visuals.material.SetColor("SpriteColor", profile.Color);

        _diceCanvasGroup.gameObject.SetActive(false);
        _diceCanvasGroup.alpha = 0.0f;
        _coinsCanvasGroup.gameObject.SetActive(false);
        _coinsCanvasGroup.alpha = 0.0f;

        HideFinalDiceNumber();
    }

    public void StartDiceRoll()
    {
        for (int i = 0; i < _diceCountMax; i++)
        {
            if (i < _diceCount) _diceCanvasGroup.transform.GetChild(i).gameObject.SetActive(true);
            else _diceCanvasGroup.transform.GetChild(i).gameObject.SetActive(false);
        }
        _currentRollingDiceIndex = 0;
        LastRolledDiceNumber = 0;

        _diceCanvasGroup.gameObject.SetActive(true);
        _diceCanvasGroup.alpha = 0.0f;

        _diceCanvasGroup.DOFade(1.0f, 0.15f);

        foreach (var text in _diceTexts)
        {
            text.SetText(
                (Random.Range(BoardManager.MinDiceNumber, BoardManager.MaxDiceNumber) + _moveCountModifier).ToString()
            );
        }

        _generateRandomNumberCoroutine = StartCoroutine(GenerateRandomNumber());
        IEnumerator GenerateRandomNumber()
        {
            while (true)
            {
                yield return _numberGenerationDelay;
                for (int i = _currentRollingDiceIndex; i < _diceCount; i++)
                {
                    _diceTexts[i].SetText(
                        (Random.Range(BoardManager.MinDiceNumber, BoardManager.MaxDiceNumber) + _moveCountModifier).ToString()
                    );
                }
            }
        }
    }

    public async Task<bool> FinishDiceRoll()
    {
        int numberRolled = Random.Range(BoardManager.MinDiceNumber, BoardManager.MaxDiceNumber);
        int diceNumber = numberRolled + _moveCountModifier;
        LastRolledDiceNumber += diceNumber;
        _diceTexts[_currentRollingDiceIndex].SetText(diceNumber.ToString());
        _currentRollingDiceIndex++;

        if (_currentRollingDiceIndex < _diceCount) return false;

        SetMoveCountModifier(0);
        SetDiceCount(_diceCountMin);
        StopCoroutine(_generateRandomNumberCoroutine);

        await Task.Delay(400);

        ShowFinalDiceNumber();
        SetFinalDiceNumberText(LastRolledDiceNumber);
        HideDice();

        return true;
    }

    public void SetFinalDiceNumberText(int number)
    {
        _finalDiceNumberText.SetText(number.ToString());
    }

    private void HideDice()
    {
        _diceCanvasGroup.DOFade(0.0f, 0.15f).OnComplete(() => _diceCanvasGroup.gameObject.SetActive(false));
    }

    private void ShowFinalDiceNumber()
    {
        _finalDiceNumberText.DOFade(1.0f, 0.15f);
    }

    public void HideFinalDiceNumber()
    {
        _finalDiceNumberText.DOFade(0.0f, 0.15f);
    }

    public async Task MoveToSpace(BoardSpace space)
    {
        StandingOnBoardSpaceId = space.Id;
        Sequence sequence = DOTween.Sequence();
        sequence.Insert(0.0f, transform.DOMoveX(space.transform.position.x, 0.3f));
        sequence.Insert(0.0f, transform.DOMoveZ(space.transform.position.z, 0.3f));
        sequence.Insert(0.0f, _visuals.transform.DOLocalMoveY(0.4f, 0.1f).SetEase(Ease.OutQuad));
        sequence.Insert(0.1f, _visuals.transform.DOLocalMoveY(0.0f, 0.4f).SetEase(Ease.OutBounce));
        await sequence.Play().AsyncWaitForCompletion();
    }

    public void SetMoveCountModifier(int modifier)
    {
        _moveCountModifier = modifier;
    }

    public void SetDiceCount(int count)
    {
        _diceCount = Mathf.Clamp(count, _diceCountMin, _diceCountMax);
    }

    public void PlayCoinsAnimation(int amount, bool addedCoins = true)
    {
        if (addedCoins)
            _coinsText.SetText($"+{amount} <sprite index=0>");
        else
            _coinsText.SetText($"<color=red>-{amount} <sprite index=0>");

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
