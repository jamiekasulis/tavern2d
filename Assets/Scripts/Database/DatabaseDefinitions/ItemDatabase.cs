using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Assets/Databases/Item Database")] // Keep this so you can create a SO instance
public class ItemDatabase : ScriptableObject
{
    public List<Item> allItems;

    public Item? GetById(string id)
    {
        return allItems.Find(i => i.id == id);
    }
}
