using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemPriceText;
    [SerializeField] private TextMeshProUGUI _itemAmountText;
    [SerializeField] private GameObject _soldOutOverlay;

    public Item Item { get; private set; }
    public int Amount { get; private set; } = 0;

    private bool SoldOut => Amount <= 0;
    private Button _button;

    public void Setup(Item item)
    {
        _button = GetComponent<Button>();

        Item = item;
        Amount = Random.Range(1, 4); // TODO: Change later
        _itemNameText.SetText(Item.Name);
        _itemIcon.sprite = Item.Icon;
        _itemPriceText.SetText($"{Item.Price} <sprite index=0>");

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
        if (SoldOut) return;

        var playerData = GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index);
        if (playerData.CoinCount < Item.Price) return;

        bool itemAdded = playerData.AddItem(Item, 1);

        if (itemAdded)
        {
            playerData.RemoveCoins(Item.Price);
            Amount--;
            UpdateSlot();
        }
    }
}
