using UnityEngine;

public class GridArea
{
    [SerializeField] public int numRows, numCols;
    [SerializeField] private Vector3 Origin; // Top right of the grid
    public GridCell[,] Cells;
    [SerializeField] public float CellSize { get; private set; }

    public GridArea(Vector3 origin, int rows, int cols, float cellSize)
    {
        Origin = origin;
        Origin.z = 1f;
        numRows = rows;
        numCols = cols;
        CellSize = cellSize;
        Cells = new GridCell[rows, cols];
        FillInCells(new Coordinate { row = 0, col = 0 }, new Coordinate { row = numRows, col = numCols });
    }

    public bool ContainsCoordinate(Coordinate coord)
    {
        return coord.row >= 0 && coord.col >= 0 && coord.row < numRows && coord.col < numCols;
    }

    /**
     * Fills in the grid from [from, to] with the given content.
     */
    public void FillInCells(Coordinate from, Coordinate to)
    {
        if (from.row >= to.row || from.col >= to.col) // Can handle this later. But for now, fail. @TODO
        {
            throw new System.Exception($"Given invalid args for 'from' and 'to' in FillInCells");
        }

        (int, int) fillSize = (Mathf.Abs(from.row - to.row), Mathf.Abs(from.col - to.col));
        if (fillSize.Item1 > numRows || fillSize.Item2 > numCols)
        {
            throw new System.Exception($"Given invalid range to fill - it's larger than the grid! Requested range: {from} - {to}");
        }

        for (int r = from.row; r < to.row; r++)
        {
            for (int c = from.col; c < to.col; c++)
            {
                GridCell cell = new GridCell(
                    new Coordinate { row=r, col=c },
                    Origin + new Vector3(CellSize * c, CellSize * r, 0)
                );
                Cells[r, c] = cell;
            }
        }
    }

    public void DrawGrid(bool outlineOnly)
    {
        DrawGridOutline();
        if (!outlineOnly)
        {
            DrawIndividualCells();
        }
    }

    private void DrawGridOutline()
    {
        Vector3 topRight = Origin + new Vector3(CellSize * numCols, 0f, 1f);
        Vector3 bottomRight = Origin + new Vector3(CellSize * numCols, CellSize * numRows, 1);
        Vector3 bottomLeft = new Vector3(0, Origin.y + CellSize * numRows, 1);

        Debug.DrawLine(Origin, topRight, Color.red, 2);
        Debug.DrawLine(Origin, bottomLeft, Color.red, 2);
        Debug.DrawLine(topRight, bottomRight, Color.red, 2);
        Debug.DrawLine(bottomLeft, bottomRight, Color.red, 2);
    }

    /**
     * A more efficient way to do this would be drawing a line for each row and column,
     * rather than 4 lines per cell. But doing it this way ensures that the cells are 
     * correctly positioned. Since this is really only going to be used for debugging,
     * it's preferable to leave it as-is.
     */
    private void DrawIndividualCells()
    {
        foreach (GridCell cell in Cells)
        {
            Vector3 o = cell.OriginWorldPosition;
            Vector3 topRight = o + new Vector3(CellSize * numCols, 0, 0);
            Vector3 bottomLeft = o + new Vector3(0, CellSize * numRows, 0);
            Vector3 bottomRight = o + new Vector3(CellSize * numCols, CellSize * numRows);
            Debug.DrawLine(o, topRight, Color.red, 2);
            Debug.DrawLine(o, bottomLeft, Color.red, 2);
            Debug.DrawLine(topRight, bottomRight, Color.red, 2);
            Debug.DrawLine(bottomLeft, bottomRight, Color.red, 2);
        }
    }
}
