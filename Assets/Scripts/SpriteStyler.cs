using UnityEngine;

/**
 * This function allows me to apply various styles to Unity Sprites. For now
 * this only includes tinting the sprite color, which is used in Build Mode to 
 * indicate the validity of the placement of some object.
 * 
 * I prefer to do it this way, where I can simply attach this
 * reusable SpriteStyler component to game objects that would need to be styled,
 * so that I don't have to have each object repeat the same styling code.
 */
public class SpriteStyler : MonoBehaviour
{
    public void Tint(SpriteRenderer renderer, Color color)
    {
        renderer.color = color;
    }
}
