using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshSwapper))]
public class PlaceableObject : MonoBehaviour
{
    public SpriteRenderer Renderer;
    private MeshSwapper MeshSwapper;

    [SerializeField] public Item item;

    private void Awake()
    {
        MeshSwapper = gameObject.GetComponent<MeshSwapper>();
        Renderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void Initialize()
    {
        Renderer.sprite = MeshSwapper.Current.sprite;
    }

    /**
     * Returns the BoundsInt representing the floor space this placeable
     * object is currently taking up.
     */
    public Bounds GetFloorGridBounds(Grid grid)
    {
        BoxCollider2D col = MeshSwapper.Current.GetComponent<BoxCollider2D>();
        return col.bounds;
    }

    /**
     * Rotates the object 90 degrees. Under the hood, this will use the
     * MeshSwapper to swap out the current mesh for the one facing in the
     * direction we rotate to.
     */
    public void Rotate(RotationDirectionEnum dir, Vector3 centerPositionWorld)
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

        MeshSwapper.LoadMeshForDirection((DirectionEnum)newSpriteDirection, centerPositionWorld);
        Renderer.sprite = MeshSwapper.Current.sprite;
    }

    public enum RotationDirectionEnum
    {
        Left, Right
    }

    public void TintSprite(Color color)
    {
        Renderer.color = color;
    }
}
