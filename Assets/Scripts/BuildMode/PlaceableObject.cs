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

    public BoundsInt GetFloorGridBounds(Grid grid)
    {
        BoundsInt gridBounds = new(
            grid.WorldToCell(collider.bounds.min),
            grid.WorldToCell(collider.bounds.size)
        );
        gridBounds.max = grid.WorldToCell(collider.bounds.max);
        return gridBounds;
    }
}
