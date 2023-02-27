using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class GridArea : MonoBehaviour
{
    [SerializeField] public int numRows, numCols;
    public GridCell[,] Cells;
    [SerializeField] private float CellSize = 1;
    

    public virtual void Awake()
    {
        transform.position.Set(transform.position.x, transform.position.y, 1f);
        Cells = new GridCell[numRows, numCols];
        FillInCells(new Coordinate { row = 0, col = 0 }, new Coordinate { row = numRows, col = numCols });
    }

    /**
     * Fills in the grid from [from, to] with the given content.
     */
    private void FillInCells(Coordinate from, Coordinate to)
    {
        if (from.row >= to.row || from.col >= to.col)
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
                    transform.position + new Vector3(CellSize * c, CellSize * r, 0)
                );
                Cells[r, c] = cell;
            }
        }
    }

    public virtual void Update()
    {
        DebugDrawGridLines();
    }

    private void DebugDrawGridLines()
    {
        for (int r = 0; r < numRows; r++)
        {
            for (int c = 0; c < numCols; c++)
            {
                if (Cells[r, c].buildable)
                {
                    // Draw a diagonal through all the buildable GridCells
                    Debug.DrawLine(Cells[r, c].OriginWorldPosition, Cells[r, c].OriginWorldPosition + new Vector3(CellSize, -CellSize, 0));
                }
            }
        }
    }

    #region Public functions

    public bool ContainsCoordinate(Coordinate coord)
    {
        return coord.row >= 0 && coord.col >= 0 && coord.row < numRows && coord.col < numCols;
    }

    /**
     * Returns the tiles belonging to the given tilemap that this GridArea overlaps with.
     * 
     * Note: Even partial overlap will result in that tile being returned.
     */
    public TileBase[] GetTiles(Tilemap tilemap, bool drawDebugLines)
    {
        BoundsInt bounds = GetGridAreaBounds();

        if (drawDebugLines)
        {
            // Draw two lines to illustrate the bounds (in the tilemap's grid) that this grid area covers
            Debug.DrawLine(bounds.position, bounds.position + new Vector3Int(bounds.size.x, 0, 0), Color.red, 2); // bottom
            Debug.DrawLine(bounds.position, bounds.position + new Vector3Int(0, +bounds.size.y, 0), Color.red, 2); // left
            Debug.DrawLine(bounds.max, bounds.max - new Vector3Int(0, bounds.size.y, 0), Color.red, 2); // right
            Debug.DrawLine(bounds.max, bounds.max - new Vector3Int(bounds.size.x, 0, 0), Color.red, 2); // top
        }

        return tilemap.GetTilesBlock(bounds);
    }

    public BoundsInt GetGridAreaBounds()
    {
        int scaleFactor = GetScaleFactor();
        Vector3Int startPosition = new( // bottom left corner
            GridUtils.GetSnappedToWorldGridCoordinate(transform.position.x * scaleFactor),
            GridUtils.GetSnappedToWorldGridCoordinate((transform.position.y - numRows * CellSize) * scaleFactor),
            0
        );
        
        Vector3Int size = new(
            Mathf.CeilToInt(numCols * CellSize * scaleFactor),
            Mathf.CeilToInt(numRows * CellSize * scaleFactor),
            0 // BoundsInts NEEDS z of at least 1 for it to contain anything!
        );
        return new(startPosition, size);
    }

    public int GetScaleFactor()
    {
        // The scale factor helps map from the size of the grid area to the size of the
        // actual game world grid. For example, the game world grid uses units of size 1 Unity unit
        // and our custom grid might be using a 0.25 cell size. We would want to multiply all positions and
        // sizes by a scale factor of 4 (0.25 * 4 = 1) to correctly map into the world space.
        return Mathf.RoundToInt(1 / CellSize);

    }

    public TileBase[] GetTiles(Tilemap tilemap)
    {
        return GetTiles(tilemap, true);
    }

    #endregion

    #region Debugging tools

    private void OnDrawGizmos()
    {
        DrawGridOutline();
        DrawGridLines();
    }

    private void DrawGridOutline()
    {
        Vector3 topRight = transform.position + new Vector3(CellSize * numCols, 0f, 0f);
        Vector3 bottomRight = transform.position + new Vector3(CellSize * numCols, -CellSize * numRows, 0f);
        Vector3 bottomLeft = transform.position + new Vector3(0, -CellSize * numRows, 0f);
        Gizmos.DrawLine(transform.position, topRight);
        Gizmos.DrawLine(transform.position, bottomLeft);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawSphere(transform.position, .1f); // Origin
    }

    private void DrawGridLines()
    {
        for (int col = 0; col < numCols; col++)
        {
            Gizmos.DrawLine(
                transform.position + new Vector3(col * CellSize, 0, 0),
                transform.position + new Vector3(col * CellSize, numRows * -CellSize, 0)
            );
        }
        for (int row = 0; row < numRows; row++)
        {
            Gizmos.DrawLine(
                transform.position + new Vector3(0, row * -CellSize, 0),
                transform.position + new Vector3(numCols * CellSize, row * -CellSize, 0)
            );
        }
    }

    #endregion
}
