using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Assets/Databases/Item Database")] // Keep this so you can create a SO instance
public class ItemDatabase : ScriptableObject
{
    public List<Item> allItems;
}
