using UnityEngine;

public abstract class MiniGame : MonoBehaviour
{
    public abstract void OnCalled();
    public abstract void OnBegin();

    protected void End() => MiniGameManager.Instance.End();
}
