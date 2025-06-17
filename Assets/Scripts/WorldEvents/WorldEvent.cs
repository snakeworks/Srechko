using System.Threading.Tasks;
using UnityEngine;

public abstract class WorldEvent : MonoBehaviour
{
    public abstract Task Apply();
}
