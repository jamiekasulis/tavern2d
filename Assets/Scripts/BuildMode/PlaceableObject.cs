using UnityEngine;
using System.Collections.Generic;

/**
 * A PlaceableObject is essentially a Mesh2D that corresponds to an Item
 * which can be placed in the world by the player.
 */
[ExecuteInEditMode]
[RequireComponent(typeof(MeshSwapper))]
public class PlaceableObject : MonoBehaviour
{
    public SpriteRenderer Renderer;
    private MeshSwapper MeshSwapper;
    public Vector3? placedPosition;

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
    public Bounds GetFloorGridBounds(GridArea buildableAreaGrid)
    {
        // To do this we need to account for the fact that the buildable area
        // grid might have a scale factor different from Unity's typical world
        // grid (where 1 unit is 1 Unity unit).
        BoxCollider2D col = MeshSwapper.Current.GetComponent<BoxCollider2D>();
        int scaleFactor = buildableAreaGrid.GetScaleFactor();
        Bounds scaledBounds = new();
        scaledBounds.min = col.bounds.min * new Vector2(scaleFactor, scaleFactor);
        scaledBounds.max = col.bounds.max * new Vector2(scaleFactor, scaleFactor);
        return scaledBounds;
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

    /**
     * Determines if the object's current position is a valid placement position
     * for the placeable object. It will check for collisions against other 
     * placeable objects in the given GridArea.
     */
    public bool PlacementIsValid(GridArea buildableAreaGrid)
    {
        Bounds objBounds = GetFloorGridBounds(buildableAreaGrid);
        BoundsInt buildBounds = buildableAreaGrid.GetGridAreaBounds();

        // (1) Ensure the object's floorspace is within the buildable area floor.
        // Object is contained within the buildable area
        // Unfortunately BoundsInt.Contains(point) has some idiosyncracies, so
        // althuogh this is verbose it is more dependable.
        if (!(
            objBounds.min.x >= buildBounds.min.x &&
            objBounds.min.y >= buildBounds.min.y &&
            objBounds.max.x <= buildBounds.max.x &&
            objBounds.max.y <= buildBounds.max.y
        ))
        {
            return false;
        }

        // (2) Ensure object does not collide with other placeable object colliders.
        List<Collider2D> overlappingColliders = new(10);
        MeshSwapper.Current.collider.OverlapCollider(new ContactFilter2D().NoFilter(), overlappingColliders);
        foreach (Collider2D col in overlappingColliders)
        {
            // Check if these belong to a placeable object
            Transform parent = col.gameObject.transform.parent;
            //bool hasPO = parent.TryGetComponent<PlaceableObject>(out PlaceableObject p);
            //if (hasPO)
            //{
            //    return false;
            //}
        }
        return true;
    }

    public void OnPlaced(Vector3 placedPosition)
    {
        this.placedPosition = placedPosition;
    }

    public void OnPickedUp()
    {
        this.placedPosition = null;
    }
}
