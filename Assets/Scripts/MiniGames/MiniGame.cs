using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGame : MonoBehaviour
{
    public abstract string Name { get; }

    public abstract void OnCalled();
    public abstract void OnBegin();

    protected void End(Dictionary<int, int> scores) => MiniGameManager.Instance.End(scores);
}
