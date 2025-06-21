using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : StaticInstance<MiniGameManager>
{
    [SerializeField] private GameObject _screenObject;
    [SerializeField] private MiniGame[] _games;
    [SerializeField] private Image _background;
    [SerializeField] private GameObject _finishedPanelObject;
    [SerializeField] private GameObject _resultsPanelObject;
    [SerializeField] private GameObject _introPanelObject;
    [SerializeField] private Image _static;
    [SerializeField] private TextMeshProUGUI _miniGameNameText;
    [SerializeField] private Transform _playerImagesPanel;
    [SerializeField] private Transform _playerScoresParent;
    [SerializeField] private Transform _coinsParent;

    public event System.Action Finished;

    private readonly TextMeshProUGUI[] _placeTexts = new TextMeshProUGUI[4]; 
    private readonly TextMeshProUGUI[] _scoreTexts = new TextMeshProUGUI[4]; 
    private readonly TextMeshProUGUI[] _coinsTexts = new TextMeshProUGUI[4]; 
    private MiniGame _current;
    private MiniGame _previous;

    protected override void Awake()
    {
        base.Awake();
        _finishedPanelObject.SetActive(false);
        _introPanelObject.SetActive(false);
        _background.DOFade(0.0f, 0.0f);
        _background.gameObject.SetActive(false);
        _screenObject.transform.localScale = Vector3.zero;
        foreach (var game in _games) game.gameObject.SetActive(false);

        for (int i = 0; i < _playerScoresParent.childCount; i++)
        {
            _placeTexts[i] = _playerScoresParent.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
            _scoreTexts[i] = _playerScoresParent.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>();
            _coinsTexts[i] = _coinsParent.GetChild(i).GetComponent<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        _screenObject.SetActive(false);
        _resultsPanelObject.SetActive(false);
    }

    public async void BeginRandom()
    {
        AudioManager.Instance.FadeOut(SoundName.BoardTheme);

        _current = _previous;
        while (_current == _previous)
            _current = _games[Random.Range(0, _games.Length)];

        _screenObject.SetActive(true);
        _current.gameObject.SetActive(true);
        _current.OnCalled();

        _previous = _current;

        _background.DOFade(0.0f, 0.0f);
        _background.gameObject.SetActive(true);
        _background.DOFade(0.75f, 0.5f);

        _static.DOFade(1.0f, 0.0f);
        _introPanelObject.SetActive(true);
        _miniGameNameText.SetText(_current.Name.ToUpper());
        _miniGameNameText.transform.localScale = Vector3.zero;

        var introPanelRect = _introPanelObject.GetComponent<RectTransform>();
        introPanelRect.anchoredPosition = new(introPanelRect.anchoredPosition.x, 0.0f);

        AudioManager.Instance.Play(SoundName.Static);

        await _screenObject.transform.DOScale(1.0f, 0.6f)
            .SetEase(Ease.OutCirc)
            .AsyncWaitForCompletion();

        await _static
            .DOFade(0.0f, 0.5f)
            .SetDelay(3.0f)
            .OnStart(() => AudioManager.Instance.FadeOut(SoundName.Static))
            .AsyncWaitForCompletion();

        await Task.Delay(500);

        _miniGameNameText.transform.DOScale(1.25f, 0.0f);
        _miniGameNameText.transform.DOScale(1.0f, 0.25f);
        AudioManager.Instance.Play(SoundName.MiniGameStart);

        await Task.Delay(1800);

        await introPanelRect
            .DOAnchorPosY(-900.0f, 0.5f)
            .SetDelay(2.0f)
            .AsyncWaitForCompletion();

        _introPanelObject.SetActive(false);

        await Task.Delay(100);

        _current.OnBegin();
    }

    public async void End(Dictionary<int, int> scores)
    {
        if (_current == null) return;

        var finishedRect = _finishedPanelObject.GetComponent<RectTransform>();
        finishedRect.anchoredPosition = new(finishedRect.anchoredPosition.x, 900.0f);

        var sortedScores = new List<KeyValuePair<int, int>>(scores);
        sortedScores.Sort((a, b) => b.Value.CompareTo(a.Value));

        for (int i = 0; i < sortedScores.Count; i++)
        {
            var playerIndex = sortedScores[i].Key;
            for (int j = 0; j < _playerImagesPanel.childCount; j++)
            {
                var child = _playerImagesPanel.GetChild(j);
                if (child.name.EndsWith(playerIndex.ToString()))
                {
                    child.SetSiblingIndex(i);
                    break;
                }
            }
        }

        _finishedPanelObject.SetActive(true);

        AudioManager.Instance.Play(SoundName.MiniGameEnd);

        await finishedRect
            .DOAnchorPos3DY(0.0f, 0.6f)
            .SetEase(Ease.OutBounce)
            .AsyncWaitForCompletion();

        _resultsPanelObject.SetActive(true);

        await Task.Delay(1200);

        for (int i = 0; i < sortedScores.Count; i++)
        {
            if (i >= PlayerManager.Instance.ControllerCount)
            {
                _coinsTexts[i].gameObject.SetActive(false);
                _scoreTexts[i].transform.parent.gameObject.SetActive(false);
            }
            else
            {
                _placeTexts[i].SetText($"#{i + 1}");
                _scoreTexts[i].SetText(sortedScores[i].Value.ToString());

                // Coin calculation
                int baseScore = sortedScores[i].Value;
                int rawCoins = Mathf.RoundToInt(Mathf.Pow(1.8f, sortedScores.Count - i) * baseScore * 0.08f);
                int coinsEarned = Mathf.RoundToInt(rawCoins / 5.0f) * 5;

                _coinsTexts[i].SetText($"+{coinsEarned} <sprite index=0>");
                GameManager.Instance.GetBoardPlayerData(sortedScores[i].Key).AddCoins(coinsEarned);

                _coinsTexts[i].gameObject.SetActive(true);
                _scoreTexts[i].transform.parent.gameObject.SetActive(true);
            }
        }

        await finishedRect
            .DOAnchorPos3DY(-900.0f, 0.6f)
            .SetEase(Ease.OutQuad)
            .AsyncWaitForCompletion();

        _finishedPanelObject.SetActive(false);

        await Task.Delay(4000);

        _background.DOFade(0.0f, 0.5f).OnComplete(() => _background.gameObject.SetActive(false));

        await _screenObject.transform
            .DOScale(0.0f, 0.6f)
            .SetEase(Ease.OutCirc)
            .AsyncWaitForCompletion();

        _finishedPanelObject.SetActive(false);
        _resultsPanelObject.SetActive(false);
        _screenObject.SetActive(false);
        _current.gameObject.SetActive(false);
        _current = null;

        Finished?.Invoke();
    }
}