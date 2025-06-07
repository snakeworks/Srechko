using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardItemSlot : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _itemAmountText;
    [SerializeField] private Sprite _emptyItemIcon;

    public event Action OnSlotPressed; 
    public Item Item { get; private set; }

    private Button _button;
    private BoardPlayerData _playerData;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonPressed);
    }

    public void UpdateSlot(BoardPlayerData playerData, Item item, int amount)
    {
        _playerData = playerData;
        Item = item;

        if (Item == null)
        {
            _itemIcon.sprite = _emptyItemIcon;
            _itemAmountText.SetText("x0");
        }
        else
        {
            _itemIcon.sprite = item.Icon;
            _itemAmountText.SetText($"x{amount}");
        }
    }

    private void OnButtonPressed()
    {
        if (Item == null)
        {
            return;
        }
        _playerData.SelectItem(Item);
        OnSlotPressed?.Invoke();
    }
}
