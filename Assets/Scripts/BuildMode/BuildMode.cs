using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildMode : MonoBehaviour
{
    public GameObject testPrefab; // object to place. @TODO Select this from Inventory.
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase baseTile, okTile, badTile;

    private BuildableGridArea buildableGridArea;
    private PlaceableObject placeableObject;

    private bool isEnabled = false;
    private GameObject instantiatedPrefab = null;
    
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
            }
            else
            {
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
        BoundsInt buildAreaBounds = buildableGridArea.GetGridAreaBounds();
        tilemap.size = buildAreaBounds.size;
        tilemap.origin = buildAreaBounds.position;
        tilemap.ResizeBounds(); // Will affect the changes done by the last 2 lines.
        tilemap.FloodFill(buildAreaBounds.position, baseTile);
        tilemap.CompressBounds(); // For efficiency purposes.
    }

    private void PaintTiles(BoundsInt area, TileBase tile)
    {
        // Whenever you clear the tilemap it resets the bounds to 0. Before filling it, we need to resize the tilemap
        // so that it will be able to fit the entire boxfill we do below.
        tilemap.BoxFill(area.position, tile, area.xMin, area.yMin, area.xMax, area.yMax);
    }

    private void UpdateObjectPosition()
    {
        Debug.Log($"Attempting to update obj position with mouseGridPos={mouseGridPosition}");
        if (mouseGridPosition.Equals(prevMouseGridPosition))
        {
            return; // Do nothing if the mouse is in the same cell
        }

        placementArea.ClampToBounds(buildableGridArea.GetGridAreaBounds());

        instantiatedPrefab.transform.position = CenterInCell(mouseWorldPosition);
        Debug.Log($"Centered object in cell. Position is now {instantiatedPrefab.transform.position}");

        // Update the tiles. Since the position has changed, un-paint any green or red tiles from the last position
        // before painting the new position green or red.
        tilemap.FloodFill(prevPlacementArea.position, baseTile);

        prevPlacementArea = placementArea;
        placementArea = GetPlaceableObjFloorBoundsGrid();
        Debug.Log($"Calculated placementArea to be {placementArea}");

        bool isWithinBuildableArea = tilemap.cellBounds.Contains(placementArea.min) && tilemap.cellBounds.Contains(placementArea.max);
        
        if (isWithinBuildableArea)
        {
            // @TODO also account for collisions with other objects.
            Debug.Log($"Placement is within buildable area. Painting OKTiles over {placementArea}");
            PaintTiles(placementArea, okTile);
        }
        else
        {
            Debug.Log($"Placement area is NOT within buildable area. Painting BadTiles over {placementArea}");
            // Boxfilling outside of our bounds can still happen if the boxfilled area's
            // starting/minimum position is outside of the bounds. Make sure to prevent this.
            PaintTiles(placementArea, badTile);
        }
        Debug.Log("Object position updated! FIN.");
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
        return placeableObject.GetFloorGridBounds(tilemap.layoutGrid);
    }
}
