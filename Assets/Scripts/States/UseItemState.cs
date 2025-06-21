using System.Threading.Tasks;

public class UseItemState : GameState
{
    public override async void OnEnter()
    {
        PlayerManager.Instance.DisableInput();

        var selectedItem = CurrentPlayerData.SelectedItem;
        await selectedItem.PerformItemAction();
        CurrentPlayerData.UseSelectedItem();

        await Task.Delay(300);

        if (selectedItem.RollDiceAfterUse) ChangeState(RollingDiceState);
        else ChangeState(NextTurnState);
    }

    public override void OnExit()
    {
    }
}