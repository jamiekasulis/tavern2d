using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class BuildMode : MonoBehaviour
{
    public static BuildMode Instance;

    public GameObject testPrefab; // object to place. @TODO Select this from Inventory.

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase baseTile;

    private BuildableGridArea buildableGridArea;
    private PlaceableObject placeableObject;
    public bool IsEnabled { get; private set; } = false;
    private GameObject instantiatedPrefab = null;
    private Vector2 mouseWorldPosition;
    private BoundsInt buildAreaBounds;

    private Color OK_COLOR = Color.green, BAD_COLOR = Color.red;

    [SerializeField] private UnityEvent buildModeToggledTrigger;

    private void Awake()
    {
        if (Instance != null & Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        buildableGridArea = gameObject.GetComponent<BuildableGridArea>();
        buildAreaBounds = buildableGridArea.GetGridAreaBounds();
        mouseWorldPosition = Vector3.zero;
    }

    void Update()
    {
        HandleToggleBuildMode();
        if (!IsEnabled)
        {
            return;
        }

        UpdateObjectPosition();
        HandleRotateObject();
        HandlePlaceObject();
    }

    private void HandleToggleBuildMode()
    {
        if (Input.GetKeyDown(MouseKeyboardControlsMapping.TOGGLE_BUILD_MODE))
        {
            IsEnabled = !IsEnabled;
            InstantiateOrDestroyPlaceableObject();

            if (IsEnabled)
            {
                OnBuildModeEnabled();
            }
            else
            {
                OnBuildModeDisabled();
            }

            buildModeToggledTrigger.Invoke();
        }
    }

    private void OnBuildModeEnabled()
    {
        FillBuildableArea();
    }

    private void OnBuildModeDisabled()
    {
        tilemap.ClearAllTiles();
    }

    private void HandleRotateObject()
    {
        if (Input.GetKeyDown(MouseKeyboardControlsMapping.ROTATE_LEFT))
        {
            placeableObject.Rotate(PlaceableObject.RotationDirectionEnum.Left, mouseWorldPosition);
        }
        else if (Input.GetKeyDown(MouseKeyboardControlsMapping.ROTATE_RIGHT))
        {
            placeableObject.Rotate(PlaceableObject.RotationDirectionEnum.Right, mouseWorldPosition);
        }
    }

    private void HandlePlaceObject()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            if (placeableObject.PlacementIsValid(buildableGridArea))
            {
                instantiatedPrefab = null;
                placeableObject = null;
                IsEnabled = false;
                OnBuildModeDisabled();
            }
            else
            {
                Debug.Log("Invalid placement!");
            }
        }
    }

    private void InstantiateOrDestroyPlaceableObject()
    {
        if (IsEnabled)
        {
            instantiatedPrefab = Instantiate(testPrefab, mouseWorldPosition, Quaternion.identity, gameObject.transform);
            placeableObject = instantiatedPrefab.GetComponent<PlaceableObject>();
            placeableObject.Initialize();
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

    private void FillBuildableArea()
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

    private void UpdateObjectPosition()
    {
        mouseWorldPosition = GetMouseWorldPosition();
        instantiatedPrefab.transform.position = CenterInCell(mouseWorldPosition);

        Bounds floorBounds = GetPlaceableObjFloorBoundsGrid();

        // Unfortunately we can't just do buildAreaBounds.Contains(floorBounds.min) && ...Contains(floorBounds.max)
        // This does not work, for whatever reason, when the z dimension is empty. I fuckin hate Unity. . .
        bool placementOK = placeableObject.PlacementIsValid(buildableGridArea);
        if (placementOK)
        {
            placeableObject.TintSprite(OK_COLOR);
        }
        else
        {
            placeableObject.TintSprite(BAD_COLOR);
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

    public Bounds GetPlaceableObjFloorBoundsGrid()
    {
        return placeableObject.GetFloorGridBounds(buildableGridArea);
    }
}
