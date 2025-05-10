using UnityEngine;

public static class Bootstrapper
{
    private static bool _wasInitialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        if (_wasInitialized)
        {
            return;
        }

        Object systems = Object.Instantiate(Resources.Load("pfb_Systems"));
        Object.DontDestroyOnLoad(systems);
        systems.name = "Systems";
        _wasInitialized = true;
    }
}
