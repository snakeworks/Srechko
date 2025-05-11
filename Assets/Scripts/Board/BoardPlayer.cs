using System;
using UnityEngine;

public class BoardPlayer : MonoBehaviour
{
    public PlayerController Controller { get; private set; }
    public int CoinCount { get; private set; }
    public event Action OnCoinCountChanged;

    private int Index => transform.GetSiblingIndex();

    private void Awake()
    {
        Controller = PlayerManager.Instance.GetPlayerController(Index);   
    }
}
