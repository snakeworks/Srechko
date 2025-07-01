using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static List<Player> All { get; private set; }
    public static Player Current { get; private set; }
    public static Player Get(int index) => All.InRange(index) ? All[index] : null;
    public BoardPlayerController BoardController;
    public bool HasControl { get; private set; }

    public void ClaimControl()
    {
        HasControl = true;
    }

    public void ReleaseControl()
    {
        HasControl = false;
    }
}
