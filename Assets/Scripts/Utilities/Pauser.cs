using UnityEngine;

public static class Pauser
{
    public static bool IsPaused => Time.timeScale == 0.0f;

    public static void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    public static void UnpauseGame()
    {
        Time.timeScale = 1.0f;
    }
}
