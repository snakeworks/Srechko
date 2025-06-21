using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BoardActionMenu : Menu
{
    [SerializeField] private BoardItemMenu _itemMenu;
    [SerializeField] private Button _useItemButton;
    [SerializeField] private VerticalLayoutGroup _buttonsLayout;

    public event Action OnDiceRollPressed;
    public event Action OnViewBoardPressed;
    public BoardItemMenu ItemMenu => _itemMenu;

    protected override void Init()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_buttonsLayout.GetComponent<RectTransform>());
        _buttonsLayout.enabled = false;

        for (int i = 0; i < _buttonsLayout.transform.childCount; i++)
        {
            var rect = _buttonsLayout.transform.GetChild(i).GetComponent<RectTransform>();
            rect.DOAnchorPosX(-270.0f, 0.0f);
        }
    }

    public void DiceRollPressed()
    {
        OnDiceRollPressed?.Invoke();
    }

    public void UseItemPressed()
    {
        if (BoardManager.Instance.CurrentPlayer.CanUseItems)
        {
            _itemMenu.Push();
        }
    }

    public void ViewBoardPressed()
    {
        OnViewBoardPressed?.Invoke();
    }

    public override void TweenOpen(Sequence sequence)
    {
        _useItemButton.interactable = BoardManager.Instance.CurrentPlayer.CanUseItems;
        if (!_useItemButton.interactable)
        {
            var colors = _useItemButton.colors;
            colors.disabledColor = new Color32(255, 255, 255, 70);
            _useItemButton.colors = colors;
        }
        else
        {
            _useItemButton.colors = _buttonsLayout.transform.GetChild(0).GetComponent<Button>().colors;
        }
        
        for (int i = 0; i < _buttonsLayout.transform.childCount; i++)
        {
            var rect = _buttonsLayout.transform.GetChild(i).GetComponent<RectTransform>();
            sequence.Insert(
                0.0f, rect
                .DOAnchorPosX(300, 0.15f)
                .SetDelay(i * 0.05f)
                .SetEase(Ease.OutQuad)
            );
        }
    }
    
    public override void TweenClose(Sequence sequence)
    {
        for (int i = 0; i < _buttonsLayout.transform.childCount; i++)
        {
            var rect = _buttonsLayout.transform.GetChild(i).GetComponent<RectTransform>();
            sequence.Insert(
                0.0f, rect
                .DOAnchorPosX(-270.0f, 0.15f)
                .SetDelay(i * 0.05f)
                .SetEase(Ease.OutQuad)
            );
        }
    }
}
