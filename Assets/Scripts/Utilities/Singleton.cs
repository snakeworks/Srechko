using UnityEngine;

/// <summary>
/// A static instance of this game object that will only persist for the scene it's in.
/// </summary>
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour 
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
    
    protected virtual void OnDestroy()
    {
        Instance = null;
    }
}

/// <summary>
/// A static instance class which only allows a single instance of itself to be created.
/// </summary>
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour 
{
    protected sealed override void Awake()
    {            
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        base.Awake();

        if (transform.parent == null) 
        {
            DontDestroyOnLoad(this);
        }

        Init();
    }

    protected abstract void Init();
}