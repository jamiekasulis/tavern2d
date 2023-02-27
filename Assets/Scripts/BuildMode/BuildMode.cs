using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildMode : MonoBehaviour
{
    public GameObject testPrefab; // object to place
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Grid grid;
    [SerializeField] private TileBase baseTile, okTile, badTile;

    private BuildableGridArea buildableGridArea;

    private bool isEnabled = false;
    private GameObject instantiatedPrefab = null;
    private PlaceableObject placeableObject;
    private Vector2 mouseWorldPosition;
    private Vector3Int mouseGridPosition;
    private Vector3Int prevMouseGridPosition;
    private BoundsInt prevPlacementArea, placementArea; // The current area on the grid where the active item would be placed

    private void Awake()
    {
        buildableGridArea = gameObject.GetComponent<BuildableGridArea>();
    }

    void Update()
    {
        HandleToggleBuildMode();
        if (!isEnabled)
        {
            return;
        }
        else
        {
            FillBuildableAreaOnEnabled();
            mouseWorldPosition = GetMouseWorldPosition();
            UpdateObjectPosition();
        }
    }

    private void HandleToggleBuildMode()
    {
        if (Input.GetKeyDown(MouseKeyboardControlsMapping.TOGGLE_BUILD_MODE))
        {
            isEnabled = !isEnabled;
            InstantiateOrDestroyPlaceableObject();

            if (isEnabled)
            {
                FillBuildableAreaOnEnabled();
                UpdateMousePositions();
                UpdateObjectPosition();
            }
            else
            {
                placementArea = new BoundsInt(0, 0, 0, 0, 0, 0);
                prevPlacementArea = new BoundsInt(0, 0, 0, 0, 0, 0);
                tilemap.ClearAllTiles();
            }
        }
    }

    private void InstantiateOrDestroyPlaceableObject()
    {
        if (isEnabled)
        {
            instantiatedPrefab = Instantiate(testPrefab, mouseWorldPosition, Quaternion.identity);
            placeableObject = instantiatedPrefab.GetComponent<PlaceableObject>();
        }
        else
        {
            if (instantiatedPrefab != null || placeableObject != null)
            {
                Destroy(instantiatedPrefab, 0);
                instantiatedPrefab = null;
                Destroy(placeableObject);
                placeableObject = null;
            }
        }
    }

    private void UpdateMousePositions()
    {
        mouseWorldPosition = GetMouseWorldPosition();
        prevMouseGridPosition = mouseGridPosition;
        mouseGridPosition = tilemap.layoutGrid.WorldToCell(mouseWorldPosition);
    }

    private void FillBuildableAreaOnEnabled()
    {
        Debug.Log($"Tilemap bounds are ${tilemap.cellBounds}");
        BoundsInt area = buildableGridArea.GetGridAreaBounds();
        tilemap.size = area.size; // Might need to stretch tilemap or else it will not boxfill the entire area.
        // Subtract 1 from x, y since boxfill is inclusive
        tilemap.BoxFill(area.position, baseTile, area.xMin, area.yMin, area.xMax - 1, area.yMax - 1);
        tilemap.CompressBounds();
        Debug.Log($"Tilemap bounds compressed to {tilemap.cellBounds}");
    }

    private void PaintTiles(BoundsInt area, TileBase tile)
    {
        // Whenever you clear the tilemap it resets the bounds to 0. Before filling it, we need to resize the tilemap
        // so that it will be able to fit the entire boxfill we do below.
        tilemap.BoxFill(area.position, tile, area.xMin, area.yMin, area.xMax, area.yMax);
    }

    private void UpdateObjectPosition()
    {
        if (mouseGridPosition.Equals(prevMouseGridPosition))
        {
            return; // Do nothing if the mouse is in the same cell
        }
        instantiatedPrefab.transform.position = CenterInCell(mouseWorldPosition);

        // Update the tiles. Since the position has changed, un-paint any green or red tiles from the last position
        // before painting the new position green or red.
        tilemap.FloodFill(prevPlacementArea.position, baseTile);

        prevPlacementArea = placementArea;
        placementArea = GetPlaceableObjFloorBoundsGrid();

        bool isWithinBuildableArea = tilemap.cellBounds.Contains(placementArea.min) && tilemap.cellBounds.Contains(placementArea.max);
        Debug.Log($"isWithinBuildableArea = {isWithinBuildableArea}");
        if (isWithinBuildableArea)
        {
            // @TODO also account for collisions with other objects.
            PaintTiles(placementArea, okTile);
        }
        else
        {
            // Boxfilling outside of our bounds can still happen if the boxfilled area's
            // starting/minimum position is outside of the bounds. Make sure to prevent this.
            placementArea.ClampToBounds(buildableGridArea.GetGridAreaBounds());
            PaintTiles(placementArea, badTile);
        }
        
    }

    private Vector3 CenterInCell(Vector3 worldPos)
    {
        Vector3Int cellPos = tilemap.layoutGrid.WorldToCell(worldPos);
        Vector3 centeredPos = new (
            cellPos.x + 0.5f * buildableGridArea.CellSize,
            cellPos.y + 0.5f * buildableGridArea.CellSize,
            0
        );
        return centeredPos;
    }

    public static Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public BoundsInt GetPlaceableObjFloorBoundsGrid()
    {
        return placeableObject.GetFloorGridBounds(tilemap.layoutGrid, buildableGridArea.GetScaleFactor());
    }
}
