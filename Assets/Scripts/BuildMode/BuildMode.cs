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
    private BoundsInt buildAreaBounds;

    private void Awake()
    {
        buildableGridArea = gameObject.GetComponent<BuildableGridArea>();
        buildAreaBounds = buildableGridArea.GetGridAreaBounds();
    }

    void Update()
    {
        HandleToggleBuildMode();
        if (!isEnabled)
        {
            return;
        }

        UpdateObjectPosition2();
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
        EnforceTilemapSize();
        tilemap.FloodFill(buildAreaBounds.position, baseTile);
    }

    /**
     * Unfortunately, the tilemap seems to keep getting resized when tiles
     * are drawn.
     * Run this before boxfilling to enforce the tilemap to have the
     * correct size.
     */
    private void EnforceTilemapSize()
    {
        tilemap.size = buildAreaBounds.size;
        tilemap.origin = buildAreaBounds.position;
        tilemap.ResizeBounds(); // Will affect the changes done by the last 2 lines.
    }

    private void PaintTiles(BoundsInt area, TileBase tile)
    {
        // Whenever you clear the tilemap it resets the bounds to 0. Before filling it, we need to resize the tilemap
        // so that it will be able to fit the entire boxfill we do below.
        EnforceTilemapSize();
        tilemap.BoxFill(area.position, tile, area.xMin, area.yMin, area.xMax, area.yMax);
    }

    //private void UpdateObjectPosition()
    //{
    //    if (mouseGridPosition.Equals(prevMouseGridPosition))
    //    {
    //        return;
    //    }

    //    placementArea.ClampToBounds(buildableGridArea.GetGridAreaBounds());

    //    instantiatedPrefab.transform.position = CenterInCell(mouseWorldPosition);

    //    // Update the tiles. Since the position has changed, un-paint any green or red tiles from the last position
    //    // before painting the new position green or red.
    //    tilemap.FloodFill(prevPlacementArea.position, baseTile);

    //    prevPlacementArea = placementArea;
    //    placementArea = GetPlaceableObjFloorBoundsGrid();

    //    bool isWithinBuildableArea = tilemap.cellBounds.Contains(placementArea.min) && tilemap.cellBounds.Contains(placementArea.max);
        
    //    if (isWithinBuildableArea)
    //    {
    //        // @TODO also account for collisions with other objects.
    //        PaintTiles(placementArea, okTile);
    //    }
    //    else
    //    {
    //        // Boxfilling outside of our bounds can still happen if the boxfilled area's
    //        // starting/minimum position is outside of the bounds. Make sure to prevent this.
    //        PaintTiles(placementArea, badTile);
    //    }
    //}

    private void UpdateObjectPosition2()
    {
        mouseWorldPosition = GetMouseWorldPosition();
        instantiatedPrefab.transform.position = CenterInCell(mouseWorldPosition);

        BoundsInt floorBounds = GetPlaceableObjFloorBoundsGrid();


        // Unfortunately we can't just do buildAreaBounds.Contains(floorBounds.min) && ...Contains(floorBounds.max)
        // This does not work, for whatever reason, when the z dimension is empty. I fuckin hate Unity. . .
        bool objIsWithinBuildableArea =
            floorBounds.min.x >= buildAreaBounds.min.x && floorBounds.min.y >= buildAreaBounds.min.y &&
            floorBounds.max.x <= buildAreaBounds.max.x && floorBounds.max.y <= buildAreaBounds.max.y;
        if (objIsWithinBuildableArea)
        {
            PaintTiles(floorBounds, okTile);
        }
        else
        {
            PaintTiles(floorBounds, badTile);
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
        return placeableObject.GetFloorGridBounds(tilemap.layoutGrid);
    }
}
