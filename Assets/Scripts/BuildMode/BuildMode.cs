using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class BuildMode : MonoBehaviour
{
    private bool isEnabled = false;
    private Tilemap tilemap;
    private Grid grid;
    public GameObject testPrefab; // object to place
    private GameObject instantiatedPrefab = null;
    private Vector2 mouseWorldPosition;
    

    private void Awake()
    {
        tilemap = gameObject.GetComponent<Tilemap>();
        grid = tilemap.layoutGrid;
    }

    void Update()
    {
        if (Input.GetKeyDown(MouseKeyboardControlsMapping.TOGGLE_BUILD_MODE))
        {
            isEnabled = !isEnabled;
            Debug.Log("Build Mode " + (isEnabled ? "enabled." : "disabled."));

            if (isEnabled)
            {
                instantiatedPrefab = Instantiate(testPrefab, mouseWorldPosition, Quaternion.identity);
            }
            else
            {
                if (instantiatedPrefab != null)
                {
                    Destroy(instantiatedPrefab.gameObject, 0);
                    instantiatedPrefab = null;
                }
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

    private void UpdateObjectPosition()
    {
        instantiatedPrefab.transform.position = mouseWorldPosition;
    }

    // @TODO Move to MouseUtils
    /**
     * Returns the world position of the mouse, or Vector3.zero if it
     * cannot be determined.
     */
    public static Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
