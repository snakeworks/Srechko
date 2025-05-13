using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardItemMenu : Menu
{
    [SerializeField] private BoardItemSlot[] _itemSlots;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;

    public event Action OnItemPressed;

    private BoardPlayerData CurrentPlayerData => GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index);

    protected override void Init()
    {
        foreach (var slot in _itemSlots)
        {
            slot.OnSlotPressed += OnItemSlotPressed;
        }
    }

    private void Update()
    {
        foreach (var slot in _itemSlots)
        {
            if (EventSystem.current.currentSelectedGameObject == slot.gameObject)
            {
                if (slot.Item == null)
                {
                    _itemNameText.SetText("NONE");
                    _itemDescriptionText.SetText("No item selected.");
                }
                else
                {
                    _itemNameText.SetText(slot.Item.Name.ToUpper());
                    _itemDescriptionText.SetText(slot.Item.Description);
                }
            }
        }   
    }

    private void OnItemSlotPressed()
    {
        OnItemPressed?.Invoke();
    }

    public override void TweenOpen(Sequence sequence)
    {
        int count = 0;
        foreach (var entry in CurrentPlayerData.Items)
        {
            _itemSlots[count].UpdateSlot(CurrentPlayerData, entry.Key, entry.Value);
            count++;
        }
    }
    
    public override void TweenClose(Sequence sequence)
    {
        foreach (var slot in _itemSlots)
        {
            slot.UpdateSlot(null, null, 0);
        }
    }
}
