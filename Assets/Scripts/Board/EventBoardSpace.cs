using System.Threading.Tasks;
using UnityEngine;

public class EventBoardSpace : BoardSpace
{
    public override async Task OnPlayerLanded()
    {
        await Task.Delay(10);
    }
}
