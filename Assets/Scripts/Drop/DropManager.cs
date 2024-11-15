using System;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    public static DropManager Instance { get; private set; }

    public event Action<ItemDropInfo> OnDropAdded = delegate { };
    public event Action<int> OnDropRemoved = delegate { }; // Измените здесь

    private int lastDropId = 1;
    private Dictionary<int, ItemDropInfo> activeDrops = new Dictionary<int, ItemDropInfo>();

    private void Awake()
    {
        Instance = this;
    }

    public void DropItem(ItemSO item, Vector3 position)
    {
        if (item == null)
        {
            Debug.LogError("Передан null вместо ItemSO в DropItem!");
            return;
        }

        var dropInfo = new ItemDropInfo
        {
            id = lastDropId++,
            item = item,
            position = position
        };
        
        activeDrops[dropInfo.id] = dropInfo; // Добавляем дроп в словарь активных дропов
        OnDropAdded?.Invoke(dropInfo);
    }

    public void RemoveDrop(int dropId)
    {
        if (activeDrops.ContainsKey(dropId))
        {
            activeDrops.Remove(dropId);
            OnDropRemoved?.Invoke(dropId); // Убедитесь, что передаете dropId
        }
    }
}

public class ItemDropInfo
{
    public int id;
    public ItemSO item;
    public Vector3 position;
}
