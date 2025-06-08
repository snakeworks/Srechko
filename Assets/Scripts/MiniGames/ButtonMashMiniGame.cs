using System;
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
    [SerializeField] private TextMeshProUGUI _mashButtonText;

    private const int _timerMilliseconds = 15000;

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

        await Task.Delay(500);
        _mashButtonText.gameObject.SetActive(true);

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
        }

        int passedTime = _timerMilliseconds;
        while (passedTime > 0)
        {
            await Task.Delay(1000);
            passedTime -= 1000;
            _timerText.SetText($"{Mathf.RoundToInt(passedTime / 1000)}");
        }

        PlayerManager.Instance.DisableInput();
        PlayerManager.Instance.OnAnyPlayerInteractPerformed -= OnInteractPressed;

        await Task.Delay(200);

        End();
    }
}
