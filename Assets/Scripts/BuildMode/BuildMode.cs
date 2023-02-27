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
            PaintTiles(buildableGridArea.GetGridAreaBounds(), baseTile, true);
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
                PaintTiles(buildableGridArea.GetGridAreaBounds(), baseTile, true);
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

    private void PaintTiles(BoundsInt area, TileBase tile, bool stretchTilemapSizeToArea = false)
    {
        // Whenever you clear the tilemap it resets the bounds to 0. Before filling it, we need to resize the tilemap
        // so that it will be able to fit the entire boxfill we do below.
        if (stretchTilemapSizeToArea)
        {
            tilemap.size = area.size;
        }
        tilemap.BoxFill(area.position, tile, area.xMin, area.yMin, area.xMax -1, area.yMax -1);
    }

    private void UpdateObjectPosition()
    {
        if (mouseGridPosition.Equals(prevMouseGridPosition))
        {
            return; // Do nothing if the mouse is in the same cell
        }
        instantiatedPrefab.transform.position = mouseWorldPosition;

        // Update the tiles. Since the position has changed, un-paint any green or red tiles from the last position
        // before painting the new position green or red.
        tilemap.FloodFill(prevPlacementArea.position, baseTile);

        prevPlacementArea = placementArea;
        placementArea = GetPlaceableObjFloorBoundsGrid();

        // @TODO Determine if you should use okTile or badTile based off of overlap detection
        tilemap.BoxFill(
            placementArea.position, okTile,
            placementArea.xMin, placementArea.yMin,
            placementArea.xMax, placementArea.yMax
        );
        
    }

    public static Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public BoundsInt GetPlaceableObjFloorBoundsGrid()
    {
        Bounds worldBounds = placeableObject.GetFloorWorldBoundsGrid();
        Vector3Int gridPosMin = tilemap.layoutGrid.WorldToCell(worldBounds.min);
        Vector3Int gridPosMax = tilemap.layoutGrid.WorldToCell(worldBounds.max);

        BoundsInt result = new BoundsInt(
            gridPosMin.x, gridPosMin.y, 0,
            gridPosMax.x, gridPosMax.y, 0
        );

        Debug.Log($"Floor bounds (grid):\tworldBounds={worldBounds}, gridMin={gridPosMin}, gridMax={gridPosMax}, result={result}");

        return result;
    }
}
