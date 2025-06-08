using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMashMiniGame : MiniGame
{
    [SerializeField] private Transform _playersParent;

    private const int _timerMilliseconds = 15000;

    public override async void OnBegin()
    {
        PlayerManager.Instance.DisableInput();

        foreach (Transform obj in _playersParent)
        {
            obj.gameObject.SetActive(false);
        }

        List<int> _pressCountList = new();
        for (int i = 0; i < PlayerManager.Instance.ControllerCount; i++)
        {
            _playersParent.GetChild(i).gameObject.SetActive(true);
            _pressCountList.Add(0);
        }

        await Task.Delay(500);

        PlayerManager.Instance.GiveOwnershipToAll();
        PlayerManager.Instance.EnableInput();
        PlayerManager.Instance.OnAnyPlayerInteractPerformed += OnInteractPressed;

        void OnInteractPressed(PlayerController controller)
        {
            _pressCountList[controller.Index]++;
        }

        await Task.Delay(_timerMilliseconds);

        PlayerManager.Instance.DisableInput();
        PlayerManager.Instance.OnAnyPlayerInteractPerformed -= OnInteractPressed;

        await Task.Delay(200);

        End();
    }
}
