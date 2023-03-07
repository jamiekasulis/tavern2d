using UnityEngine.Tilemaps;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))] // BoxCollider2D defines the floor space the object takes up.
public class PlaceableObject : MonoBehaviour
{
    private BoxCollider2D collider;
    [SerializeField] private Tilemap tilemap;

    private void Awake()
    {
        collider = gameObject.GetComponent<BoxCollider2D>();
    }

    public Bounds GetFloorWorldBounds()
    {
        return collider.bounds;
    }

    public BoundsInt GetFloorGridBounds(Grid grid)
    {
        Bounds worldBounds = GetFloorWorldBounds();
        BoundsInt gridBounds = new(
            grid.WorldToCell(worldBounds.min),
            grid.WorldToCell(worldBounds.size)
        );
        return gridBounds;
    }
}
