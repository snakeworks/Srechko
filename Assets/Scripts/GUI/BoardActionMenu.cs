using System;
using DG.Tweening;
using UnityEngine;

public class BoardActionMenu : Menu
{
    [SerializeField] private BoardItemMenu _itemMenu;

    public event Action OnDiceRollPressed;
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
        _itemMenu.Push();
    }

    public override void TweenClose(Sequence sequence)
    {
        
    }

    public override void TweenOpen(Sequence sequence)
    {
        
    }
}
