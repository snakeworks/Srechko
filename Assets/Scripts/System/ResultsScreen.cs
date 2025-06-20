using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsScreen : MonoBehaviour
{
    [SerializeField] private Image[] _playerImages;
    [SerializeField] private string[] _placeHexColors;
    [SerializeField] private TextMeshProUGUI _playerWinsText;
    [SerializeField] private TextMeshProUGUI _promptText;
    [SerializeField] private RectTransform _layoutGroup;

    private void Awake()
    {
        var results = PlayerManager.Instance.MatchResults;

        _playerWinsText.SetText($"PLAYER {results[0].Index + 1} WINS!");
        _playerWinsText.alpha = 0.0f;
        _promptText.alpha = 0.0f;

        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < _playerImages.Length; i++)
        {
            if (i >= PlayerManager.Instance.ControllerCount)
            {
                _playerImages[i].gameObject.SetActive(false);
                continue;
            }

            var placeText = _playerImages[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var coinsText = _playerImages[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            _playerImages[i].color = PlayerManager.Instance.GetPlayerProfile(results[i].Index).Color;

            placeText.SetText($"<color={_placeHexColors[i]}>#{results[i].Place + 1}");
            coinsText.SetText($"{results[i].Coins} <sprite index=0>");

            _playerImages[i].transform.DOScale(0.0f, 0.0f);
            sequence.Append(
                _playerImages[PlayerManager.Instance.ControllerCount - i - 1].transform
                    .DOScale(1.0f, 0.5f)
                    .SetEase(Ease.OutBounce)
                    .SetDelay(0.3f)
            );
        }

        sequence
            .Play()
            .SetDelay(0.8f)
            .OnUpdate(() => LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup))
            .OnComplete(OnTweenFinished);
    }

    private void OnTweenFinished()
    {
        _playerWinsText.DOFade(1.0f, 0.5f);
        _promptText.DOFade(1.0f, 0.5f);

        PlayerManager.Instance.GiveOwnershipTo(PlayerManager.Instance.MainPlayerController);
        PlayerManager.Instance.EnableInput();

        PlayerManager.Instance.MainPlayerController.InteractPerformed += OnInteract;

        static void OnInteract()
        {
            PlayerManager.Instance.MainPlayerController.InteractPerformed -= OnInteract;
            SceneLoader.Load(Scene.Title, showLoadingScreen: false);
        }
    }
}
