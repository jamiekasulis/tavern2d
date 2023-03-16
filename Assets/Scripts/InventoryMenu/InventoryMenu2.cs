using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class InventoryMenu2 : MonoBehaviour
{

    [SerializeField] private VisualTreeAsset CellTemplate;
    [SerializeField] private VisualTreeAsset GridRowTemplate;
    [SerializeField] private string MenuTitle;

    [SerializeField] private UnityEvent<Inventory, List<(int, ItemQuantity)>> rearrangeInventoryTrigger;
}
