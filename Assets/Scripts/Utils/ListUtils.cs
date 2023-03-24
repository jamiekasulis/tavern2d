using System.Collections.Generic;

public class ListUtils
{
    /**
     * Converts the 2D matrix into a 1D list.
     * Goes by increasing row-col order (i.e. 
     * (0,1), (0,2), (0,3), ..., (1, 0)...)
     */
    public static List<T> FlattenToList<T>(T[,] matrix)
    {
        List<T> flat = new List<T>(matrix.Length);
        foreach (var elt in matrix)
        {
            flat.Add(elt);
        }
        return flat;
    }
}
