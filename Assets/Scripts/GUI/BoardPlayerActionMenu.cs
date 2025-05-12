using System;
using DG.Tweening;
using UnityEngine;

public class BoardPlayerActionMenu : Menu
{
    public event Action OnDiceRollPressed;

    protected override void Init()
    {
        
    }

    public void PushWithBoardPlayerData(BoardPlayerData playerData)
    {
        MenuNavigator.Push(this);
    }

    public void DiceRollPressed()
    {
        OnDiceRollPressed?.Invoke();
    }

    public override void TweenClose(Sequence sequence)
    {
        
    }

    public override void TweenOpen(Sequence sequence)
    {
        
    }
}
