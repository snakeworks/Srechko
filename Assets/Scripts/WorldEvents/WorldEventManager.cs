using System.Threading.Tasks;
using UnityEngine;

public class WorldEventManager : StaticInstance<WorldEventManager>
{
    [SerializeField] private WorldEvent[] _worldEvents;

    private WorldEvent _previous;

    public async Task ApplyRandom()
    {
        WorldEvent wEvent = _previous;
        while (wEvent == _previous) wEvent = _worldEvents[Random.Range(0, _worldEvents.Length)];
        _previous = wEvent;
        await wEvent.Apply();
    }
}
