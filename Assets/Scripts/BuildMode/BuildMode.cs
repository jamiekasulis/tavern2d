using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildMode : MonoBehaviour
{
    public GameObject testPrefab; // object to place. @TODO Select this from Inventory.
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase baseTile, okTile, badTile;

    private Vector3Int mouseGridPosition;

    private BuildableGridArea buildableGridArea;
    private PlaceableObject placeableObject;

    private bool isEnabled = false;
    private GameObject instantiatedPrefab = null;
    
    private Vector2 mouseWorldPosition;
    private BoundsInt buildAreaBounds;

    private Color BAD_PLACEMENT_COLOR = Color.red;
    private Color OK_PLACEMENT_COLOR = Color.green;

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

        UpdateObjectPosition();
        HandleRotateObject();
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

    private void HandleRotateObject()
    {
        if (Input.GetKeyDown(MouseKeyboardControlsMapping.ROTATE_LEFT))
        {
            placeableObject.Rotate(PlaceableObject.RotationDirectionEnum.Left);
        }
        else if (Input.GetKeyDown(MouseKeyboardControlsMapping.ROTATE_RIGHT))
        {
            placeableObject.Rotate(PlaceableObject.RotationDirectionEnum.Right);
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
        mouseGridPosition = tilemap.layoutGrid.WorldToCell(mouseWorldPosition);
    }

    private void FillBuildableAreaOnEnabled()
    {
        EnforceTilemapSize();
        PaintTiles(buildAreaBounds, baseTile, true);
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

    /**
     * This function works -- use it when needed!
     */
    private void PaintTiles(BoundsInt area, TileBase tile, bool floodFill = false)
    {
        // Whenever you clear the tilemap it resets the bounds to 0.
        // Before filling it, we need to resize the tilemap
        // so that it will be able to fit the entire boxfill we do below.
        EnforceTilemapSize();
        if (floodFill)
        {
            tilemap.FloodFill(area.min, tile);
        }
        else
        {
            tilemap.BoxFill(area.position, tile, area.xMin, area.yMin, area.xMax, area.yMax);
        }
    }

    private void UpdateObjectPosition()
    {
        mouseWorldPosition = GetMouseWorldPosition();
        instantiatedPrefab.transform.position = CenterInCell(mouseWorldPosition);

        if (placeableObject.IsContainedBy(buildAreaBounds, tilemap.layoutGrid))
        {
            // Actually, don't tint the object at all so they can see how it will
            // really look in the space.
             placeableObject.TintSprite(Color.white);
        }
        else
        {
            placeableObject.TintSprite(BAD_PLACEMENT_COLOR);
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
