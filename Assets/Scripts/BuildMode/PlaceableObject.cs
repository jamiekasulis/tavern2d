using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))] // BoxCollider2D defines the floor space the object takes up.
public class PlaceableObject : MonoBehaviour
{
    private BoxCollider2D collider;
    private SpriteRenderer renderer;
    [SerializeField] public Item item;

    private void Awake()
    {
        collider = gameObject.GetComponent<BoxCollider2D>();
        renderer = gameObject.GetComponent<SpriteRenderer>();
        renderer.sprite = item.spriteFront;
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

    /**
     * Rotates the object 90 degrees.
     */
    public void Rotate(RotationDirectionEnum dir)
    {
        // FRONT - 0
        // LEFT - 1
        // BACK - 2
        // RIGHT - 3
        int initialSpriteDirection =
            renderer.sprite == item.spriteFront ? 0
            : renderer.sprite == item.spriteLeft ? 1
            : renderer.sprite == item.spriteBack ? 2
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

        if (newSpriteDirection == 0)
        {
            renderer.sprite = item.spriteFront;
        }
        else if (newSpriteDirection == 1)
        {
            renderer.sprite = item.spriteLeft;
        }
        else if (newSpriteDirection == 2)
        {
            renderer.sprite = item.spriteBack;
        }
        else
        {
            renderer.sprite = item.spriteRight;
        }

        collider.size = new(collider.size.y, collider.size.x); // Swap extents to "rotate" the collider without affecting the sprite

        /* 
         * @TODO Close but not quite! We are basically trying to
         * shift the entire object down, in order to move the collider down,
         * so that the bottom of the collider is at the same y level as
         * the sprite's pivot point.
         */
        gameObject.transform.position = new Vector3(
            gameObject.transform.position.x,
            renderer.transform.TransformPoint(renderer.sprite.pivot).y,
            gameObject.transform.position.z
        );
    }

    public enum RotationDirectionEnum
    {
        Left, Right
    }

    public void TintSprite(Color color)
    {
        renderer.color = color;
    }

    /**
     * Returns whether the floor space of this placeable object is fully
     * within the given bounds.
     */
    public bool IsContainedBy(BoundsInt area, Grid grid)
    {
        BoundsInt floorBounds = GetFloorGridBounds(grid);
        
        // Unfortunately we can't just do
        // buildAreaBounds.Contains(floorBounds.min) && ...Contains(floorBounds.max)
        // This does not work, for whatever reason, when the z dimension is empty.
        // So instead we check that the min and max point's x and y coords
        // are contained within the area.
        return floorBounds.min.x >= area.min.x && floorBounds.min.y >= area.min.y &&
            floorBounds.max.x <= area.max.x && floorBounds.max.y <= area.max.y;
    }
}
