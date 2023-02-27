using UnityEngine;

public class GridCell
{
    public Coordinate GridPosition { get; private set; } // The position of this cell in the grid
    public Vector3 OriginWorldPosition { get; private set; } // Origin is the top-left of the cell

    // metadata
    public bool buildable = false;

    public GridCell(Coordinate gridPos, Vector3 worldPos)
    {
        GridPosition = gridPos;
        OriginWorldPosition = worldPos;
    }
}

public struct Coordinate
{
    public int row, col;

    public override string ToString()
    {
        return $"({row},{col})";
    }
}
