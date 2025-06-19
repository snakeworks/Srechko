using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BoardPlayerSelectionMenu : Menu
{
    [SerializeField] private Button _boardPlayerButtonPrefab;
    [SerializeField] private Transform _buttonCreationParent;

    private event Action<int> _boardPlayerButtonPressed;

    protected override void Init()
    {
        for (int i = 0; i < PlayerManager.Instance.ControllerCount; i++)
        {
            var playerButton = Instantiate(_boardPlayerButtonPrefab, _buttonCreationParent);
            playerButton.transform.GetChild(0).GetComponent<Image>().color
                = PlayerManager.Instance.GetPlayerProfile(i).Color;
            int playerIndex = i;
            playerButton.GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    OnBoardPlayerButtonPressed(playerIndex);
                }
            );
        }
    }

    public async Task<BoardPlayerController> GetSelectedPlayer()
    {
        Push();
        _boardPlayerButtonPressed += OnPressed;

        BoardPlayerController pressedOnBoardPlayer = null;
        PlayerManager.Instance.EnableInput();

        void OnPressed(int index)
        {
            _boardPlayerButtonPressed -= OnPressed;
            pressedOnBoardPlayer = BoardManager.Instance.GetBoardPlayerControllerAt(index);
            ForcePop();
        }

        while (IsOnStack)
        {
            await Task.Delay(1);
        }
        PlayerManager.Instance.DisableInput();

        return pressedOnBoardPlayer;
    }

    private void OnBoardPlayerButtonPressed(int index)
    {
        _boardPlayerButtonPressed?.Invoke(index);
    }

    public override void TweenOpen(Sequence sequence)
    {
    }

    public override void TweenClose(Sequence sequence)
    {
    }
}
