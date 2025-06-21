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
    [SerializeField] private CanvasGroup _errorPopupCanvasGroup;
    [SerializeField] private TextMeshProUGUI _errorText;

    public event Action OnItemPressed;

    private BoardPlayerData CurrentPlayerData => GameManager.Instance.GetBoardPlayerData(BoardManager.Instance.CurrentPlayer.Index);

    protected override void Init()
    {
        foreach (var slot in _itemSlots)
        {
            slot.OnSlotPressed += OnItemSlotPressed;
        }
        _errorPopupCanvasGroup.alpha = 0.0f;
    }

    private void Update()
    {
        foreach (var slot in _itemSlots)
        {
            if (EventSystem.current.currentSelectedGameObject == slot.gameObject)
            {
                if (slot.Item == null)
                {
                    _itemNameText.SetText("EMPTY");
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

    private void OnItemSlotPressed(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            PopupError(message);
            return;
        }
        OnItemPressed?.Invoke();
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
