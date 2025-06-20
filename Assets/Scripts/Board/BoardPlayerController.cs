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
    [SerializeField] private ParticleSystem _sleepParticles;
    [SerializeField] private TextMeshProUGUI _popupText;
    [SerializeField] private TextMeshProUGUI[] _diceTexts = new TextMeshProUGUI[_diceCountMax];
    [SerializeField] private TextMeshProUGUI _finalDiceNumberText;
    [SerializeField] private DirectionalPromptDefinition[] _directionalPrompts;

    public int StandingOnBoardSpaceId { get; private set; } = -1;
    public bool SkipNextTurn { get; private set; } = false;
    public bool CanUseItems { get; private set; } = true;
    public Transform CameraView => _cameraView;
    public int Index => transform.GetSiblingIndex();
    public int LastRolledDiceNumber { get; private set; } = -1;

    private int _moveCountModifier = 0;
    private int _diceCount = _diceCountMin;
    private int _currentRollingDiceIndex = 0;
    private readonly WaitForSeconds _numberGenerationDelay = new(0.025f);
    private Coroutine _generateRandomNumberCoroutine;
    private readonly Dictionary<BoardSpace.Direction, DirectionalPromptDefinition> _directionalPromptsDict = new();
    private float _defaultDirectionalPromptScale;
    private int _turnSkipCount = 0;
    private int _turnCanUseItemCount = 0;
    private const int _diceCountMin = 1;
    private const int _diceCountMax = 3;

    private void Awake()
    {
        var profile = PlayerManager.Instance.GetPlayerProfile(Index);
        _visuals.material.SetColor("SpriteColor", profile.Color);

        _diceCanvasGroup.gameObject.SetActive(false);
        _diceCanvasGroup.alpha = 0.0f;
        _popupText.alpha = 0.0f;
        _popupText.gameObject.SetActive(false);

        foreach (var def in _directionalPrompts)
        {
            _defaultDirectionalPromptScale = def.Sprite.transform.localScale.x;
            _directionalPromptsDict.Add(def.Direction, def);
            def.Sprite.DOFade(0.0f, 0.0f);
        }

        _sleepParticles.Stop();

        HideFinalDiceNumber();
    }

    public void OnTryTurn()
    {
        if (SkipNextTurn)
        {
            _turnSkipCount++;
            if (_turnSkipCount >= 2)
            {
                _turnSkipCount = 0;
                SetSkipNextTurn(false);
            }
        }
        else if (!CanUseItems)
        {
            _turnCanUseItemCount++;
            if (_turnCanUseItemCount >= 2)
            {
                _turnCanUseItemCount = 0;
                SetCanUseItems(true);
            }
        }
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
                (Random.Range(BoardManager.MinDiceNumber, BoardManager.MaxDiceNumber + 1) + _moveCountModifier).ToString("D2")
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
                        (Random.Range(BoardManager.MinDiceNumber, BoardManager.MaxDiceNumber + 1) + _moveCountModifier).ToString("D2")
                    );
                }
                if (!AudioManager.Instance.IsPlaying(SoundName.DiceRoll))
                    AudioManager.Instance.Play(SoundName.DiceRoll);
            }
        }
    }

    public async Task<bool> FinishDiceRoll()
    {
        int numberRolled = Random.Range(BoardManager.MinDiceNumber, BoardManager.MaxDiceNumber+1);
        int diceNumber = numberRolled + _moveCountModifier;
        LastRolledDiceNumber += diceNumber;
        _diceTexts[_currentRollingDiceIndex].SetText(diceNumber.ToString());

        _diceTexts[_currentRollingDiceIndex].transform.parent.DOScale(1.3f, 0.0f);
        _diceTexts[_currentRollingDiceIndex].transform.parent.DOScale(1.0f, 0.1f);

        _currentRollingDiceIndex++;

        AudioManager.Instance.Play(SoundName.DiceRollFinished);

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
        if (StandingOnBoardSpaceId >= 0) BoardSpace.Get(StandingOnBoardSpaceId).OnPlayerExited(this);
        StandingOnBoardSpaceId = space.Id;
        space.OnPlayerEntered(this);

        Sequence sequence = DOTween.Sequence();
        sequence.Insert(0.0f, transform.DOMoveX(space.transform.position.x, 0.3f));
        sequence.Insert(0.0f, transform.DOMoveZ(space.transform.position.z, 0.3f));
        sequence.Insert(0.0f, _visuals.transform.DOLocalMoveY(0.4f, 0.1f).SetEase(Ease.OutQuad));
        sequence.Insert(0.1f, _visuals.transform.DOLocalMoveY(0.0f, 0.4f).SetEase(Ease.OutBounce));

        AudioManager.Instance.Play(SoundName.PlayerLand, delay: 0.2f);

        await sequence.Play().AsyncWaitForCompletion();
    }

    public void UpdateVisualsOffset(Vector3 offset)
    {
        _visuals.transform.DOLocalMoveX(offset.x, 0.1f);
        _visuals.transform.DOLocalMoveZ(offset.z, 0.1f);
    }

    public void ShowDirectionalPrompts(BoardSpace.Direction[] directions)
    {
        foreach (var dir in directions)
        {
            // Resetting 
            _directionalPromptsDict[dir].Sprite.DOFade(0.0f, 0.0f);
            _directionalPromptsDict[dir].Sprite.transform.DOScale(_defaultDirectionalPromptScale, 0.0f);

            _directionalPromptsDict[dir].Sprite.DOFade(1.0f, 0.1f);
        }
    }

    public async Task HideDirectionalPrompts(BoardSpace.Direction chosenDirection)
    {
        Sequence sequence = DOTween.Sequence();
        foreach (var prompt in _directionalPrompts)
        {
            if (prompt.Direction == chosenDirection)
            {
                sequence.Insert(0.0f, prompt.Sprite.transform.DOScale(1.1f, 0.3f));
                sequence.Insert(0.0f, prompt.Sprite.DOFade(0.0f, 0.1f).SetDelay(0.15f));
            }
            else
            {
                sequence.Insert(0.0f, prompt.Sprite.DOFade(0.0f, 0.1f));
            }
        }
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

    public void SetSkipNextTurn(bool skip)
    {
        if (skip) _turnSkipCount = 0;
        SkipNextTurn = skip;
        if (skip) _sleepParticles.Play();
        else _sleepParticles.Stop();
    }

    public void SetCanUseItems(bool can)
    {
        if (!can) _turnCanUseItemCount = 0;
        CanUseItems = can;
    }

    public async Task PlayPopupAnimation(string text)
    {
        _popupText.SetText(text);
        _popupText.gameObject.SetActive(true);

        _popupText.alpha = 0.0f;
        var rectTransform = _popupText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new(
            rectTransform.anchoredPosition.x,
            -230.0f
        );

        Sequence sequence = DOTween.Sequence();

        sequence.Insert(0.0f, _popupText.DOFade(1.0f, 0.1f));
        sequence.Insert(0.0f, rectTransform.DOAnchorPosY(0.0f, 0.1f));
        sequence.Insert(0.0f, _popupText.DOFade(0.0f, 0.1f)
            .SetDelay(1.0f)
            .OnComplete(() => _popupText.gameObject.SetActive(false)));

        await sequence.Play().AsyncWaitForCompletion();
    }

    public async Task PlayCoinsAnimation(int amount, bool positive = true)
    {
        if (positive)
        {
            AudioManager.Instance.Play(SoundName.CoinsGain);
            await PlayPopupAnimation($"<color=yellow>+{amount} <sprite index=0>");
        }
        else
        {
            AudioManager.Instance.Play(SoundName.CoinsLose);
            await PlayPopupAnimation($"<color=red>-{amount} <sprite index=0>");
        }
    }

    public async Task PlayMoveCountAnimation(int amount, bool positive = true)
    {
        if (positive)
        {
            AudioManager.Instance.Play(SoundName.EnergyGain);
            await PlayPopupAnimation($"+{amount} <sprite index=1>");
        }
        else
        {
            AudioManager.Instance.Play(SoundName.EnergyLose);
            await PlayPopupAnimation($"<color=red>-{amount} <sprite index=1>");
        }
    }

    [System.Serializable]
    public class DirectionalPromptDefinition
    {
        [SerializeField] public BoardSpace.Direction Direction;
        [SerializeField] public SpriteRenderer Sprite;
    }
}
