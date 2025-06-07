using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardPlayerData : MonoBehaviour
{
    public int Index => transform.GetSiblingIndex();
    public int CoinCount { get; private set; }
    public event Action OnCoinCountChanged;
    public Dictionary<Item, int> Items { get; private set; } = new();
    public Item SelectedItem { get; private set; }
    public event Action<Item, int> OnItemUpdate;
    public const int MaxUniqueItemCount = 3;

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

    public bool AddItem(Item item, int amount)
    {
        if (Items.ContainsKey(item))
        {
            Items[item] += amount;
        }
        else
        {
            if (Items.Count >= MaxUniqueItemCount)
            {
                return false;
            }
            Items.Add(item, amount);
        }
        OnItemUpdate?.Invoke(item, Items[item]);
        return true;
    }

    public void SelectItem(Item item)
    {
        if (item == null || !Items.ContainsKey(item))
        {
            return;
        }
        SelectedItem = item;
    }

    public void UseSelectedItem()
    {
        Items[SelectedItem]--;
        OnItemUpdate?.Invoke(SelectedItem, Items[SelectedItem]);
        if (Items[SelectedItem] <= 0)
        {
            Items.Remove(SelectedItem);
        }
        SelectedItem = null;
    }
}
