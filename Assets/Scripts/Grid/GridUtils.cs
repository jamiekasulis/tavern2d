using UnityEngine;

public class GridUtils
{

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
