using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class QuickDrawMiniGame : MiniGame
{
    [SerializeField] private GameObject _waitText;
    [SerializeField] private GameObject _goPanel;
    [SerializeField] private Transform _playersParent;
    [SerializeField] private Transform _placesParent;

    public override string Name => "Quick Draw";

    private const int _roundCount = 5;

    public override void OnCalled()
    {
        PlayerManager.Instance.DisableInput();
        _waitText.SetActive(false);
        _goPanel.SetActive(false);
        foreach (Transform child in _playersParent)
        {
            child.gameObject.SetActive(true);
        }
        foreach (Transform child in _placesParent)
        {
            child.gameObject.SetActive(false);
        }
    }

    public override async void OnBegin()
    {
        List<int> _pointsList = new();
        for (int i = 0; i < PlayerManager.Instance.ControllerCount; i++)
        {
            _pointsList.Add(0);
        }

        for (int i = 0; i < _roundCount; i++)
        {
            foreach (Transform obj in _placesParent)
            {
                obj.gameObject.SetActive(false);
            }

            _waitText.SetActive(true);
            _goPanel.SetActive(false);
        
            List<PlayerController> controllerDrawOrder = new();
            List<PlayerController> failedDraws = new();

            float randomWaitTime = Random.Range(1.0f, 8.0f);
            bool isWaiting = true;

            PlayerManager.Instance.EnableInput();
            PlayerManager.Instance.GiveOwnershipToAll();

            PlayerManager.Instance.OnAnyPlayerInteractPerformed += OnPressed;

            AudioManager.Instance.Play(SoundName.QuickDrawTheme);
            await Awaitable.WaitForSecondsAsync(randomWaitTime);
            AudioManager.Instance.Stop(SoundName.QuickDrawTheme);
            isWaiting = false;

            AudioManager.Instance.Play(SoundName.Whistle);
            _waitText.SetActive(false);
            _goPanel.SetActive(true);

            void OnPressed(PlayerController controller)
            {
                if (controllerDrawOrder.Contains(controller) || failedDraws.Contains(controller)) return;

                _playersParent.GetChild(controller.Index).GetChild(0).DOKill();
                _playersParent
                    .GetChild(controller.Index)
                    .GetChild(0).localScale = new(0.85f, 0.85f, 0.85f);
                _playersParent.GetChild(controller.Index).GetChild(0).DOScale(1.0f, 0.2f);

                if (isWaiting)
                {
                    failedDraws.Add(controller);
                    AudioManager.Instance.Play(SoundName.Error);
                    return;
                }

                _pointsList[controller.Index] += 12 * (_pointsList.Count - controllerDrawOrder.Count);
                controllerDrawOrder.Add(controller);

                AudioManager.Instance.Play(SoundName.DiceRollFinished);
            }

            float passedTime = 0;
            while (controllerDrawOrder.Count != PlayerManager.Instance.ControllerCount)
            {
                await Awaitable.EndOfFrameAsync();
                passedTime += Time.deltaTime;
                if (passedTime > 1.0f) break;
            }

            PlayerManager.Instance.DisableInput();
            PlayerManager.Instance.OnAnyPlayerInteractPerformed -= OnPressed;

            for (int j = 0; j < PlayerManager.Instance.ControllerCount; j++)
            {
                var controller = PlayerManager.Instance.GetPlayerController(j);
                if (!controllerDrawOrder.Contains(controller)
                    && !failedDraws.Contains(controller))
                {
                    failedDraws.Add(controller);       
                }
            }

            for (int j = 0; j < controllerDrawOrder.Count; j++)
            {
                var controller = controllerDrawOrder[j];
                _placesParent.GetChild(controller.Index).gameObject.SetActive(true);
                _placesParent.GetChild(controller.Index).GetChild(0).GetComponent<TextMeshProUGUI>().SetText($"#{j + 1}");
            }

            for (int j = 0; j < failedDraws.Count; j++)
            {
                var controller = failedDraws[j];
                _placesParent.GetChild(controller.Index).gameObject.SetActive(true);
                _placesParent.GetChild(controller.Index).GetChild(0).GetComponent<TextMeshProUGUI>().SetText("DNF");
            }

            await Awaitable.WaitForSecondsAsync(1.5f);
        }

        End(new Dictionary<int, int>
        {
            {0, _pointsList.InRange(0) ? _pointsList[0] : 0},
            {1, _pointsList.InRange(1) ? _pointsList[1] : 0},
            {2, _pointsList.InRange(2) ? _pointsList[2] : 0},
            {3, _pointsList.InRange(3) ? _pointsList[3] : 0},
        });
    }
}
