using System.Threading.Tasks;
using UnityEngine;

public class WorldEventManager : StaticInstance<WorldEventManager>
{
    [SerializeField] private WorldEvent[] _worldEvents;

    public async Task ApplyRandom()
    {
        WorldEvent wEvent = _worldEvents[Random.Range(0, _worldEvents.Length)];
        await wEvent.Apply();
    }
}
