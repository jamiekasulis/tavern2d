using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    public string id;
    public string itemName;
    public string description;
    public Sprite sprite;
    public bool buildMode; // Whether or not the item can be placed by the player in Build Mode
}
