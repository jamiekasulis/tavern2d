using UnityEngine;

[RequireComponent(typeof(MeshSwapper))]
public class PlaceableObject : MonoBehaviour
{
    private SpriteRenderer Renderer;
    private MeshSwapper MeshSwapper;
    private Mesh2D Mesh;

    [SerializeField] public Item item;

    private void Awake()
    {
        MeshSwapper = gameObject.GetComponent<MeshSwapper>();
        Renderer = gameObject.GetComponent<SpriteRenderer>();

        Mesh = MeshSwapper.Default;
        Renderer.sprite = Mesh.sprite;
    }

    /**
     * Returns the BoundsInt representing the floor space this placeable
     * object is currently taking up.
     */
    public BoundsInt GetFloorGridBounds(Grid grid)
    {
        BoxCollider2D col = Mesh.GetComponent<BoxCollider2D>();
        BoundsInt gridBounds = new(
            grid.WorldToCell(col.bounds.min),
            grid.WorldToCell(col.bounds.size)
        );
        gridBounds.max = grid.WorldToCell(col.bounds.max);
        return gridBounds;
    }

    /**
     * Rotates the object 90 degrees. Under the hood, this will use the
     * MeshSwapper to swap out the current mesh for the one facing in the
     * direction we rotate to.
     */
    public void Rotate(RotationDirectionEnum dir)
    {
        // FRONT - 0
        // LEFT - 1
        // BACK - 2
        // RIGHT - 3
        int initialSpriteDirection =
            Renderer.sprite == item.spriteFront ? 0
            : Renderer.sprite == item.spriteLeft ? 1
            : Renderer.sprite == item.spriteBack ? 2
            : 3;

        int newSpriteDirection = initialSpriteDirection
            + (dir == RotationDirectionEnum.Left ? -1 : 1);

        if (newSpriteDirection > 3)
        {
            newSpriteDirection = 0;
        }
        else if (newSpriteDirection < 0)
        {
            newSpriteDirection = 3;
        }

        Mesh = MeshSwapper.GetMeshForDirection((DirectionEnum)newSpriteDirection);
        Renderer.sprite = Mesh.sprite;
    }

    public enum RotationDirectionEnum
    {
        Left, Right
    }
}
