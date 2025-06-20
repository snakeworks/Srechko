using System;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardPlayerSelectionMenu : Menu
{
    [SerializeField] private Button _boardPlayerButtonPrefab;
    [SerializeField] private Transform _buttonCreationParent;
    [SerializeField] private RectTransform _itemPanel;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _itemText;

    private event Action<int> BoardPlayerButtonPressed;

    protected override void Init()
    {
        for (int i = 0; i < PlayerManager.Instance.ControllerCount; i++)
        {
            var playerButton = Instantiate(_boardPlayerButtonPrefab, _buttonCreationParent);
            playerButton.transform.GetChild(0).GetComponent<Image>().color
                = PlayerManager.Instance.GetPlayerProfile(i).Color;
            playerButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText($"PLAYER {i+1}");
            int playerIndex = i;
            playerButton.GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    OnBoardPlayerButtonPressed(playerIndex);
                }
            );
        }
        _firstSelectedButton = GetComponentInChildren<Button>();
        LastSelectedObject = _firstSelectedButton.gameObject;
    }

    public async Task<BoardPlayerController> GetSelectedPlayer()
    {
        Push();
        BoardPlayerButtonPressed += OnPressed;

        BoardPlayerController pressedOnBoardPlayer = null;
        PlayerManager.Instance.EnableInput();

        async void OnPressed(int index)
        {
            if (index == BoardManager.Instance.CurrentPlayer.Index)
            {
                await ModalMenu.PushOk("You cannot choose yourself.");
                ModalMenu.ForcePop();
                return;
            }
            BoardPlayerButtonPressed -= OnPressed;
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
        BoardPlayerButtonPressed?.Invoke(index);
    }

    public override void TweenOpen(Sequence sequence)
    {
        var item = GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index).SelectedItem;
        _itemIcon.sprite = item.Icon;
        _itemText.SetText($"Use Item: {item.Name}");
        _itemPanel.transform.DOScale(1.15f, 0.0f);
        _itemPanel.transform.DOScale(1.0f, 0.2f);
    }

    public override void TweenClose(Sequence sequence)
    {
    }
}
