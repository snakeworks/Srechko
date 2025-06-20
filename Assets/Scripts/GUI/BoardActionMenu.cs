using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BoardActionMenu : Menu
{
    [SerializeField] private BoardItemMenu _itemMenu;
    [SerializeField] private Button _useItemButton;

    public event Action OnDiceRollPressed;
    public event Action OnViewBoardPressed;
    public BoardItemMenu ItemMenu => _itemMenu;

    protected override void Init()
    {
        
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
    }
    
    public override void TweenClose(Sequence sequence)
    {

    }
}
