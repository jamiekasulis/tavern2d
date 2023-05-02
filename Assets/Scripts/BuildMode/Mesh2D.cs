using UnityEngine;

/**
 * A Mesh2D is my own representation of a mesh in 2D space. This is really just
 * a Sprite and a collider.
 * 
 * It is called Mesh2D to distinguish it from Unity's general concept of a Mesh.
 * 
 * Using Mesh2D allows us to do things like rotate a sprite (the visual
 * representation) while also rotating its physical representation (bounding
 * boxes and such).
 */
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Sprite))]
public class Mesh2D : MonoBehaviour
{
    public Sprite sprite;
    public BoxCollider2D collider { get; private set; }
    public DirectionEnum direction;

    private void Awake()
    {
        collider = gameObject.GetComponent<BoxCollider2D>();
    }
}
