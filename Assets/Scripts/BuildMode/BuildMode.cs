using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteStyler))]
public class BuildMode : MonoBehaviour
{
    public static BuildMode Instance;

    private GameObject objectToPlacePrefab;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase baseTile;
    private SpriteStyler spriteStyler;

    private BuildableGridArea buildableGridArea;
    private PlaceableObject placeableObject;
    public bool IsEnabled { get; private set; } = false;
    private GameObject instantiatedPrefab = null;
    private Vector2 mouseWorldPosition;
    private BoundsInt buildAreaBounds;

    private PlaceableObject mousedOverPlaceableObject;

    // Styling
    private Color OK_COLOR = Color.green, BAD_COLOR = Color.red, MOUSEOVER_COLOR = new Color(244, 255, 0, 1);

    /**
     * When build mode is toggled, style or unstyle the Inventory UI accordingly.
     */
    [SerializeField] private UnityEvent buildModeToggledTrigger;
    /**
     * When an object is picked up, put it in the player inventory if there's space.
     */
    [SerializeField] private UnityEvent<Item, int> PlaceItemInPlayerInventoryCallback;
    /**
     * Redraw the inventory after making changes to it.
     */
    [SerializeField] private UnityEvent<Inventory> redrawPlayerInventoryCallback;
    /**
     * This callback is responsible for making sure the Inventoy backend is updated
     * with the correct inventory data.
     */
    [SerializeField] private UnityEvent<Inventory, List<(int, ItemQuantity)>> ReflectChangesToInventoryBackendCallback;


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
        spriteStyler = gameObject.GetComponent<SpriteStyler>();
    }

    void Update()
    {
        /*
         * Note: Check for key presses in this function and dispatch to the correct
         * handler accordingly.
         * This prevents multiple functions acting on the same key press occurring
         * within the same frame.
         * 
         * In general, we want to check the input and current state to see if a
         * set of functionality is relevant before calling that function.
         *
         * Let's also try to keep the most expensive functions from being executed 
         * when they do not need to be.
         */
        if (Input.GetKeyDown(MouseKeyboardControlsMapping.TOGGLE_BUILD_MODE))
        {
            HandleToggleBuildMode();
            if (!IsEnabled)
            {
                return;
            }
        }

        // Get the object we're working with (if any)
        if (placeableObject == null && objectToPlacePrefab != null)
        {
            InstantiatePlaceableObject();
        }
        else if (placeableObject == null)
        {
            HandleMouseoverPlacedObject();
        }

        // Left click: Can either be (1) placing an object, (2) picking up
        // a placed object
        // Note: The flow of if's and if-else's is very important here. We should
        // allow ourselves to always check for mouse clicks.
        if (Input.GetMouseButtonDown(0))
        {
            if (placeableObject != null)
            {
                HandlePlaceObject();
            }
            else if (placeableObject == null && mousedOverPlaceableObject != null)
            {
                HandlePickUpPlacedObject();
            }
        }
        // Right click: Picks up object and immediately places in player
        // inventory
        else if (Input.GetMouseButtonDown(1))
        {
            if (mousedOverPlaceableObject != null && placeableObject == null)
            {
                HandlePickUpPlacedObjectToInventory();
            }
        } else if (Input.GetKeyDown(MouseKeyboardControlsMapping.CANCEL_GENERAL))
        {
            HandleCancelPlacement();
        }
        
        UpdateObjectPosition();
        HandleRotateObject();
    }

    private void HandleToggleBuildMode()
    {
        if (Input.GetKeyDown(MouseKeyboardControlsMapping.TOGGLE_BUILD_MODE))
        {
            IsEnabled = !IsEnabled;

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
        if (mousedOverPlaceableObject != null)
        {
            spriteStyler.Tint(mousedOverPlaceableObject.Renderer, Color.white); // Untint
        }
        if (placeableObject != null)
        {
            DestroyPlaceableObject();
        }
    }

    private void HandleMouseoverPlacedObject()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        PlaceableObject? firstEligibleHit = null;
        if (mousedOverPlaceableObject != null)
        {
            spriteStyler.Tint(mousedOverPlaceableObject.Renderer, Color.white); // untint
            mousedOverPlaceableObject = null;
        }

        foreach (var hit in hits)
        {
            PlaceableObject po = hit.collider.gameObject.GetComponentInParent<PlaceableObject>(); // Mesh2D will be hit by the ray. It has a PO parent.
            if (po != null)
            {
                if (po.placedPosition != null)
                {
                    firstEligibleHit = po;
                    break;
                }
            }
        }

        if (firstEligibleHit != null)
        {
            mousedOverPlaceableObject = firstEligibleHit;
            spriteStyler.Tint(firstEligibleHit.Renderer, MOUSEOVER_COLOR);   
        }
    }

    /**
     * Places item back into inventory.
     */
    private void HandlePickUpPlacedObjectToInventory()
    {
        if (mousedOverPlaceableObject == null || placeableObject != null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            PlaceItemInPlayerInventoryCallback.Invoke(mousedOverPlaceableObject.item, 1);
            Destroy(mousedOverPlaceableObject.gameObject, 0);
            mousedOverPlaceableObject = null;
            objectToPlacePrefab = null;
        }
    }

    /**
     * Allows you to reposition the item.
     */
    private void HandlePickUpPlacedObject()
    {
        if (mousedOverPlaceableObject == null || placeableObject != null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            placeableObject = mousedOverPlaceableObject;
            instantiatedPrefab = mousedOverPlaceableObject.gameObject;
            objectToPlacePrefab = placeableObject.item.prefab;
        }
    }

    public void HandleCancelPlacement()
    {
        if (placeableObject == null)
        {
            return;
        }
        if (placeableObject.placedPosition != null)
        {
            // Return to its original spot
            placeableObject.transform.position = placeableObject.placedPosition ?? Vector3.zero; // Vector3.zero case should not happen. ?? is to satisfy compiler.
            spriteStyler.Tint(placeableObject.Renderer, Color.white);
            placeableObject = null;
            instantiatedPrefab = null;
            objectToPlacePrefab = null;
        }
        else
        {
            // Return to inventory
            PlaceInPlayerInventory(placeableObject.item, 1);
            placeableObject.OnPickedUp();
            Destroy(placeableObject.gameObject, 0);
            placeableObject = null;
            objectToPlacePrefab = null;
            instantiatedPrefab = null;
        }
    }

    public void PlaceInPlayerInventory(Item item, int quantity)
    {
        Inventory playerInv = InventoryManager.Instance.PlayerInventory;
        if (playerInv.ContainsItem(item) || playerInv.HasEmptySpace())
        {
            ItemQuantity iq = new() { item = item, quantity = quantity };
            int idx = playerInv.Add(iq);
            redrawPlayerInventoryCallback.Invoke(playerInv);

            ItemQuantity iqInInv = playerInv.Get(idx);
            ReflectChangesToInventoryBackendCallback.Invoke(playerInv, new List<(int, ItemQuantity?)> { (idx, iqInInv) });
        }
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
        if (placeableObject.PlacementIsValid(buildableGridArea))
        {
            PlaceObject();
        }
        else
        {
            Debug.Log("Invalid placement!");
        }
    }

    private void PlaceObject()
    {
        placeableObject.OnPlaced(placeableObject.transform.position);
        spriteStyler.Tint(placeableObject.Renderer, Color.white);
        
        placeableObject = null;
        instantiatedPrefab = null;
        objectToPlacePrefab = null;
        mousedOverPlaceableObject = null;
    }

    private void InstantiatePlaceableObject()
    {
        instantiatedPrefab = Instantiate(objectToPlacePrefab, mouseWorldPosition, Quaternion.identity, gameObject.transform);
        placeableObject = instantiatedPrefab.GetComponent<PlaceableObject>();
        placeableObject.Initialize();
    }

    private void DestroyPlaceableObject()
    {
        Destroy(placeableObject.gameObject, 0);
        instantiatedPrefab = null;
        placeableObject = null;
        objectToPlacePrefab = null;
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

    private void UpdateObjectPosition()
    {
        if (placeableObject == null)
        {
            return;
        }

        mouseWorldPosition = GetMouseWorldPosition();
        placeableObject.transform.position = CenterInCell(mouseWorldPosition);
            
        // Unfortunately we can't just do buildAreaBounds.Contains(floorBounds.min) && ...Contains(floorBounds.max)
        // This does not work, for whatever reason, when the z dimension is empty. I fuckin hate Unity. . .
        bool placementOK = placeableObject.PlacementIsValid(buildableGridArea);
        if (placementOK)
        {
            spriteStyler.Tint(placeableObject.Renderer, OK_COLOR);
        }
        else
        {
            spriteStyler.Tint(placeableObject.Renderer, BAD_COLOR);
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

    public void SetObjectToPlace(Item item)
    {
        if (!item.buildMode)
        {
            throw new System.Exception($"Attemted to set Build Mode's object to place to a non-build mode approved item! {item}");
        }
        objectToPlacePrefab = item.prefab;
        Debug.Log($"Set objectToPlacePrefab to {item.prefab.name}");
    }
}
