using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ShopItemSlot : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemPriceText;
    [SerializeField] private TextMeshProUGUI _itemAmountText;
    [SerializeField] private Transform _panelTransform;
    [SerializeField] private GameObject _soldOutOverlay;

    public Item Item { get; private set; }
    public int Amount { get; private set; } = 0;

    private bool SoldOut => Amount <= 0;
    private Button _button;
    private ShopMenu _shopMenu;

    public void Setup(Item item, ShopMenu menu)
    {
        _button = GetComponent<Button>();
        _shopMenu = menu;

        Item = item;
        Amount = Random.Range(Item.ShopMinCount, Item.ShopMaxCount+1);
        _itemNameText.SetText(Item.Name);
        _itemIcon.sprite = Item.Icon;
        _itemPriceText.SetText(
            Item.Price <= 0 ? "FREE" : $"{Item.Price} <sprite index=0>"
        );

        UpdateSlot();

        _button.onClick.AddListener(OnButtonPressed);
    }

    public void UpdateSlot()
    {
        _itemAmountText.SetText($"x{Amount}");
        _soldOutOverlay.SetActive(SoldOut);
    }

    private void OnButtonPressed()
    {
        if (SoldOut)
        {
            _shopMenu.PopupError("This item has been sold out!");
            return;
        }

        var playerData = GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index);
        if (Item.Price > 0 && playerData.CoinCount < Item.Price)
        {
            _shopMenu.PopupError("You do not have enough coins!");
            return;
        }

        bool itemAdded = playerData.AddItem(Item);

        if (itemAdded)
        {
            _panelTransform.DOKill();
            _panelTransform.DOScale(0.8f, 0.0f);
            _panelTransform.DOScale(1.0f, 0.2f);

            if (Item.Price > 0)
            {
                playerData.RemoveCoins(Item.Price);
            }

            Amount--;
            UpdateSlot();
            AudioManager.Instance.Play(SoundName.BuyItem);
        }
        else
        {
            _shopMenu.PopupError("Your inventory is full!");
        }
    }
}
