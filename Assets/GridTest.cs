using System.Collections.Generic;
using UnityEngine;

public class GridTest : MonoBehaviour
{
    public GridArea myGrid;

    private void Awake()
    {
        
    }

    private void Update()
    {
        myGrid = new GridArea(Vector3.zero, 3, 4, 1f);
        myGrid.DrawGrid(false);
    }
}
