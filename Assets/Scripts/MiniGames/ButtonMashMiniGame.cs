using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ButtonMashMiniGame : MiniGame
{
    [SerializeField] private Transform _playersParent;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _mashButtonText;

    public override string Name => "Button Mash";

    private const int _timerMilliseconds = 10000;

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
        _mashButtonText.gameObject.SetActive(false);
    }

    public override async void OnBegin()
    {
        List<int> _pressCountList = new();
        for (int i = 0; i < PlayerManager.Instance.ControllerCount; i++)
        {
            _pressCountList.Add(0);
        }

        _mashButtonText.gameObject.SetActive(true);

        AudioManager.Instance.Play(SoundName.ButtonMashTheme);

        async void FlashMashButton(float fade)
        {
            if (!_mashButtonText.IsActive()) return;
            await _mashButtonText
                .DOFade(fade, 0.02f)
                .AsyncWaitForCompletion();
            FlashMashButton(fade == 1.0f ? 0.3f : 1.0f);
        }

        FlashMashButton(0.5f);

        PlayerManager.Instance.GiveOwnershipToAll();
        PlayerManager.Instance.EnableInput();
        PlayerManager.Instance.OnAnyPlayerInteractPerformed += OnInteractPressed;

        void OnInteractPressed(PlayerController controller)
        {
            _pressCountList[controller.Index]++;
            _playersParent.GetChild(controller.Index).GetChild(0).DOKill();
            _playersParent
                .GetChild(controller.Index)
                .GetChild(0).localScale = new(0.85f, 0.85f, 0.85f);
            _playersParent.GetChild(controller.Index).GetChild(0).DOScale(1.0f, 0.2f);
            AudioManager.Instance.Play(SoundName.Impact);
        }

        int passedTime = _timerMilliseconds;
        while (passedTime > 0)
        {
            await Task.Delay(1000);
            passedTime -= 1000;
            _timerText.SetText($"{Mathf.RoundToInt(passedTime / 1000)}");
        }

        _mashButtonText.gameObject.SetActive(false);

        PlayerManager.Instance.DisableInput();
        PlayerManager.Instance.OnAnyPlayerInteractPerformed -= OnInteractPressed;

        AudioManager.Instance.Stop(SoundName.ButtonMashTheme);

        End(new Dictionary<int, int>
        {
            {0, _pressCountList.InRange(0) ? _pressCountList[0] : 0},
            {1, _pressCountList.InRange(1) ? _pressCountList[1] : 0},
            {2, _pressCountList.InRange(2) ? _pressCountList[2] : 0},
            {3, _pressCountList.InRange(3) ? _pressCountList[3] : 0},
        });
    }
}
