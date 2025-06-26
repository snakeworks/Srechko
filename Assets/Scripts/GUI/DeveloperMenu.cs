using DG.Tweening;
using UnityEngine;

public class DeveloperMenu : Menu
{
    protected override void Init()
    {
    }

    public override void TweenOpen(Sequence sequence) { }
    public override void TweenClose(Sequence sequence) { }

    public void TryPush()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameState.ChoosingBoardActionState) return;
        Push();
    }

    public void OnSkipTurn()
    {
        GameManager.Instance.ChangeState(GameState.NextTurnState);
        MenuNavigator.ForcePopUntilEmpty();
    }

    public void OnSkipRound()
    {
        while (BoardManager.Instance.CurrentPlayerTurnIndex != -1)
        {
            BoardManager.Instance.NextTurn();
        }
        GameManager.Instance.ChangeState(GameState.NextTurnState);
        MenuNavigator.ForcePopUntilEmpty();
    }

    public void OnEndGame()
    {
        while (BoardManager.Instance.CurrentRound <= BoardManager.MaxRoundCount)
        {
            BoardManager.Instance.NextTurn();
        }
        GameManager.Instance.ChangeState(GameState.NextTurnState);
        MenuNavigator.ForcePopUntilEmpty();
    }
}
