using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class ShopMenu : Menu
{
    [SerializeField] private ShopItemSlot _shopItemSlotPrefab;
    [SerializeField] private Transform _shopItemSlotsCreationParent;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private CanvasGroup _errorPopupCanvasGroup;
    [SerializeField] private TextMeshProUGUI _errorText;
    [SerializeField] private TextMeshProUGUI _restockText;

    private Item[] _sellingItems;
    private int _restockCount = 0;
    private int _restockFrequency;
    private GameObject _restockSign;
    private readonly List<ShopItemSlot> _itemSlots = new();

    protected override void Init()
    {
        _restockFrequency = Random.Range(3, 8);
        _restockFrequency = Mathf.Clamp(_restockFrequency, 0, BoardManager.MaxRoundCount - BoardManager.Instance.CurrentRound);
        DisableOnStartup = false;
    }

    private void Start()
    {
        Restock();
        _errorPopupCanvasGroup.alpha = 0.0f;
        BoardManager.Instance.OnNextRound += OnNextRound;
        gameObject.SetActive(false);
    }

    public void Setup(GameObject restockSign)
    {
        _restockSign = restockSign;
        _restockSign.SetActive(false);
    }

    private void Update()
    {
        foreach (var slot in _itemSlots)
        {
            if (EventSystem.current.currentSelectedGameObject == slot.gameObject)
            {
                if (slot.Item == null)
                {
                    _itemDescriptionText.SetText("No item selected.");
                }
                else
                {
                    _itemDescriptionText.SetText(slot.Item.Description);
                }
            }
        }
    }

    private void OnNextRound()
    {
        _restockCount++;
        if (_restockCount >= _restockFrequency)
        {
            Restock();
        }
    }

    private void Restock()
    {
        if (BoardManager.Instance.CurrentRound > 1)
            _restockSign.SetActive(true);

        _sellingItems = new Item[4];

        _restockCount = 0;
        foreach (var child in _itemSlots)
        {
            Destroy(child.gameObject);
        }
        _itemSlots.Clear();
        for (int i = 0; i < _sellingItems.Length; i++)
        {
            var randomItem = ItemRegistry.Instance.GetRandom();
            while (_sellingItems.Contains(randomItem))
            {
                randomItem = ItemRegistry.Instance.GetRandom();
            }
            _sellingItems[i] = randomItem;
            var slot = Instantiate(_shopItemSlotPrefab).GetComponent<ShopItemSlot>();
            slot.transform.SetParent(_shopItemSlotsCreationParent);
            _itemSlots.Add(slot);
            slot.Setup(_sellingItems[i], this);
        }
        _firstSelectedButton = _itemSlots[0].GetComponent<Button>();
        LastSelectedObject = _firstSelectedButton.gameObject;
    }

    public void PopupError(string text)
    {
        AudioManager.Instance.Play(SoundName.Error);

        _errorText.SetText(text);
        _errorPopupCanvasGroup.DOKill();

        _errorPopupCanvasGroup.alpha = 1.0f;
        _errorPopupCanvasGroup.transform.DOScale(1.15f, 0.0f);

        _errorPopupCanvasGroup.transform.DOScale(1.0f, 0.2f);
        _errorPopupCanvasGroup.DOFade(0.0f, 0.5f).SetDelay(1.0f);
    }

    public override void TweenOpen(Sequence sequence)
    {
        var playerController = PlayerManager.Instance.GetPlayerController(BoardManager.Instance.CurrentPlayer.Index);
        playerController.CancelPerformed += TryCloseMenu;
        var roundsToRestock = _restockFrequency - _restockCount;
        _restockText.SetText($"Restocks in: {roundsToRestock} {(roundsToRestock <= 1 ? "Round" : "Rounds")}");
    }

    public override void TweenClose(Sequence sequence)
    {
        var playerController = PlayerManager.Instance.GetPlayerController(BoardManager.Instance.CurrentPlayer.Index);
        playerController.CancelPerformed -= TryCloseMenu;
        _restockSign.SetActive(false);
    }

    private async void TryCloseMenu()
    {
        var result = await ModalMenu.PushYesNo("Leave the shop?");
        if (result == ModalMenu.Result.Yes) MenuNavigator.ForcePopUntilEmpty();
        else ModalMenu.ForcePop();
    }
}
