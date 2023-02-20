using UnityEngine;

/**
 * Specifies the size of a grid, i.e. in the UI.
 * This is used so that we can configure this in the editor
 * for any UIs using grids.
 * This allows us to use a UI script for the grid which assumes 
 * a constant int for row and col size (which is required by compiler)
 * to create the cells array.
 */
public class GridSizeSpecification : MonoBehaviour
{
    [SerializeField] private int numRows, numCols;

    public int GetNumRows()
    {
        return numRows;
    }

    public int GetNumCols()
    {
        return numCols;
    }
}
