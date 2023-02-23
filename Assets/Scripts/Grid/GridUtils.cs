using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUtils
{
    // @TODO
    //public static Coordinate WorldToGridArea(Vector3 worldPos, GridArea grid)
    //{
        
    //}

    public static int GetSnappedToWorldGridCoordinate(float n)
    {
        if (n == 0)
        {
            return (int)n;
        }
        else if (n < 0)
        {
            return Mathf.FloorToInt(n);
        }
        else
        {
            return Mathf.CeilToInt(n);
        }
    }
}
