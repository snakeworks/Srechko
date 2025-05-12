using System;
using UnityEngine;

public class BoardPlayerData : MonoBehaviour
{
    public int Index => transform.GetSiblingIndex();
    public int CoinCount { get; private set; }
    public event Action OnCoinCountChanged;

    private void Awake()
    {
    }

    public void AddCoins(int amount)
    {
        CoinCount += amount;
        OnCoinCountChanged?.Invoke();
    }

    public void RemoveCoins(int amount)
    {
        CoinCount -= amount;
        OnCoinCountChanged?.Invoke();
    }
}
