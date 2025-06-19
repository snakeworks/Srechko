using UnityEngine;

public class ItemRegistry : StaticInstance<ItemRegistry>
{
    [SerializeField] private Item[] _items;

    public Item GetRandom()
    {
        return _items[Random.Range(0, _items.Length)];
    }
}
