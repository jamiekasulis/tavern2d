using UnityEngine;
using UnityEngine.Tilemaps;

public class GridArea : MonoBehaviour
{
    [SerializeField] public int numRows, numCols;
    public GridCell[,] Cells;
    [SerializeField] private float CellSize = 1;
    [SerializeField] private Tilemap BuildableAreaTilemap;

    private void Awake()
    {
        transform.position.Set(transform.position.x, transform.position.y, 1f);
        Cells = new GridCell[numRows, numCols];
        FillInCells(new Coordinate { row = 0, col = 0 }, new Coordinate { row = numRows, col = numCols });
    }

    public bool ContainsCoordinate(Coordinate coord)
    {
        return coord.row >= 0 && coord.col >= 0 && coord.row < numRows && coord.col < numCols;
    }

    /**
     * Fills in the grid from [from, to] with the given content.
     */
    private void FillInCells(Coordinate from, Coordinate to)
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
                    transform.position + new Vector3(CellSize * c, CellSize * r, 0)
                );
                Cells[r, c] = cell;
            }
        }
    }

    private void Update()
    {
        Debug.Log($"Found {GetTiles(BuildableAreaTilemap).Length} tiles in GridArea");
    }

    /**
     * Returns the tiles from the world grid which this custom grid
     * overlaps with.
     * 
     * Note: Even partial overlap will result in that tile being returned.
     */
    public TileBase[] GetTiles(Tilemap tilemap, bool drawDebugLines)
    {

        int GetSnappedToWorldGridCoordinate(float n)
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

        bool xIsPos = transform.position.x > 0, yIsPos = transform.position.y > 0;

        Vector3Int startPosition = new ( // bottom left corner
            GetSnappedToWorldGridCoordinate(transform.position.x),
            GetSnappedToWorldGridCoordinate(transform.position.y - numRows * CellSize),
            (int)tilemap.layoutGrid.transform.position.z
        );
        Vector3Int size = new(
            Mathf.CeilToInt(numCols * CellSize),
            Mathf.CeilToInt(numRows * CellSize),
            0
        );
        BoundsInt bounds = new(startPosition, size);

        if (drawDebugLines)
        {
            // Draw two lines to illustrate the bounds (in the tilemap's grid) that this grid area covers
            Debug.DrawLine(bounds.position, bounds.position + new Vector3Int(bounds.size.x, 0, 0), Color.red, 2);
            Debug.DrawLine(bounds.position, bounds.position + new Vector3Int(0, +bounds.size.y, 0), Color.red, 2);
        }

        TileBase[] tiles = tilemap.GetTilesBlock(bounds);
        return tiles;
    }

    public TileBase[] GetTiles(Tilemap tilemap)
    {
        return GetTiles(tilemap, true);
    }

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
