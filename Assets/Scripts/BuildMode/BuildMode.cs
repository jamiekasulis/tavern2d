using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildMode : MonoBehaviour
{
    public GameObject testPrefab; // object to place
    [SerializeField] private Tilemap buildModeTilemap;
    [SerializeField] private Grid buildModeGrid;
    [SerializeField] private Tilemap BuildableAreaTilemap;
    [SerializeField] private TileBase baseTile, okTile, badTile;

    private BuildableGridArea buildableGridArea;

    private bool isEnabled = false;
    private GameObject instantiatedPrefab = null;
    private Vector2 mouseWorldPosition;
    

    private void Awake()
    {
        buildableGridArea = gameObject.GetComponent<BuildableGridArea>();
    }

    void Update()
    {
        if (Input.GetKeyDown(MouseKeyboardControlsMapping.TOGGLE_BUILD_MODE))
        {
            isEnabled = !isEnabled;
            Debug.Log("Build Mode " + (isEnabled ? "enabled." : "disabled."));

            InstantiateOrDestroyPlaceableObject();
            if (isEnabled)
            {
                PaintBuildableAreaTiles();
            }
        }

        if (!isEnabled)
        {
            return;
        }

        mouseWorldPosition = GetMouseWorldPosition();
        UpdateObjectPosition();

        if (Input.GetMouseButton(0))
        {
            // Place object
            // Reparent to BuildableAreaGrid
            // Set instantiatedPrefab to null
        }
    }

    private void InstantiateOrDestroyPlaceableObject()
    {
        if (isEnabled)
        {
            instantiatedPrefab = Instantiate(testPrefab, mouseWorldPosition, Quaternion.identity);
        }
        else
        {
            if (instantiatedPrefab != null)
            {
                Destroy(instantiatedPrefab, 0);
                instantiatedPrefab = null;
            }
        }
    }

    private void PaintBuildableAreaTiles()
    {
        BoundsInt gridBounds = buildableGridArea.GetGridAreaBounds();
        Debug.Log($"Discovered grid bounds to be {gridBounds}");

        buildModeTilemap.BoxFill(gridBounds.position, baseTile, gridBounds.xMin, gridBounds.yMin, gridBounds.xMax, gridBounds.yMax);
    }

    private void UpdateObjectPosition()
    {
        instantiatedPrefab.transform.position = mouseWorldPosition;
    }

    public static Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
