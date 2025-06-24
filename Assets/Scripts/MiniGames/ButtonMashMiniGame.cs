using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMashMiniGame : MiniGame
{
    [SerializeField] private Transform _playersParent;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private CanvasGroup _mashButtonCanvasGroup;
    [SerializeField] private Image _promptImage;
    [SerializeField] private Sprite[] _promptSprites;

    public override string Name => "Button Mash";

    private const int _timerMilliseconds = 10000;
    private int _currentButtonIndex = 0;

    public override void OnCalled()
    {
        PlayerManager.Instance.DisableInput();
        _timerText.SetText($"{Mathf.RoundToInt(_timerMilliseconds / 1000)}");
        for (int i = 0; i < _playersParent.childCount; i++)
        {
            if (i < PlayerManager.Instance.ControllerCount)
                _playersParent.GetChild(i).gameObject.SetActive(true);
            else
                _playersParent.GetChild(i).gameObject.SetActive(false);
        }
        _mashButtonCanvasGroup.gameObject.SetActive(false);
    }

    public override async void OnBegin()
    {
        List<int> _pressCountList = new();
        for (int i = 0; i < PlayerManager.Instance.ControllerCount; i++)
        {
            _pressCountList.Add(0);
        }

        _mashButtonCanvasGroup.gameObject.SetActive(true);

        AudioManager.Instance.Play(SoundName.ButtonMashTheme);

        async void FlashMashButton(float fade)
        {
            if (!_mashButtonCanvasGroup.gameObject.activeInHierarchy) return;
            await _mashButtonCanvasGroup
                .DOFade(fade, 0.02f)
                .AsyncWaitForCompletion();
            FlashMashButton(fade == 1.0f ? 0.3f : 1.0f);
        }

        FlashMashButton(0.5f);

        PlayerManager.Instance.GiveOwnershipToAll();
        PlayerManager.Instance.EnableInput();

        PlayerManager.Instance.OnAnyPlayerPromptSouthPerformed += OnInputSouth;
        PlayerManager.Instance.OnAnyPlayerPromptEastPerformed += OnInputEast;
        PlayerManager.Instance.OnAnyPlayerPromptWestPerformed += OnInputWest;
        PlayerManager.Instance.OnAnyPlayerPromptNorthPerformed += OnInputNorth;

        void OnInputSouth(PlayerController controller) => OnPressed(controller, 0);
        void OnInputEast(PlayerController controller) => OnPressed(controller, 1);
        void OnInputWest(PlayerController controller) => OnPressed(controller, 2);
        void OnInputNorth(PlayerController controller) => OnPressed(controller, 3);

        void OnPressed(PlayerController controller, int index)
        {
            if (index != _currentButtonIndex) return;
            _pressCountList[controller.Index]++;
            _playersParent.GetChild(controller.Index).GetChild(0).DOKill();
            _playersParent
                .GetChild(controller.Index)
                .GetChild(0).localScale = new(0.85f, 0.85f, 0.85f);
            _playersParent.GetChild(controller.Index).GetChild(0).DOScale(1.0f, 0.2f);
            AudioManager.Instance.Play(SoundName.Impact);
        }

        int passedTime = _timerMilliseconds;
        int nextPromptTime = 0;
        while (passedTime > 0)
        {
            await Task.Delay(1000);
            passedTime -= 1000;
            nextPromptTime += 1000;
            if (nextPromptTime > 2000)
            {
                nextPromptTime = 0;
                RandomizeButton();
            }
            _timerText.SetText($"{Mathf.RoundToInt(passedTime / 1000)}");
        }

        _mashButtonCanvasGroup.gameObject.SetActive(false);

        PlayerManager.Instance.DisableInput();

        PlayerManager.Instance.OnAnyPlayerPromptSouthPerformed -= OnInputSouth;
        PlayerManager.Instance.OnAnyPlayerPromptEastPerformed -= OnInputEast;
        PlayerManager.Instance.OnAnyPlayerPromptWestPerformed -= OnInputWest;
        PlayerManager.Instance.OnAnyPlayerPromptNorthPerformed -= OnInputNorth;

        AudioManager.Instance.Stop(SoundName.ButtonMashTheme);

        End(new Dictionary<int, int>
        {
            {0, _pressCountList.InRange(0) ? _pressCountList[0] : 0},
            {1, _pressCountList.InRange(1) ? _pressCountList[1] : 0},
            {2, _pressCountList.InRange(2) ? _pressCountList[2] : 0},
            {3, _pressCountList.InRange(3) ? _pressCountList[3] : 0},
        });
    }

    private void RandomizeButton()
    {
        int newIndex = Random.Range(0, 4);
        while (newIndex == _currentButtonIndex) newIndex = Random.Range(0, 4);
        _currentButtonIndex = newIndex;
        _promptImage.sprite = _promptSprites[_currentButtonIndex];
    }
}
