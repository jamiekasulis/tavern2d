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
        int degrees = dir == RotationDirectionEnum.Left ? 90 : -90;
        //gameObject.transform.Rotate(new(0, 0, degrees));

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

        Debug.Log($"initialSpriteDirection={initialSpriteDirection}, newSpriteDirection={newSpriteDirection}");
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
        
    }

    public enum RotationDirectionEnum
    {
        Left, Right
    }
}
