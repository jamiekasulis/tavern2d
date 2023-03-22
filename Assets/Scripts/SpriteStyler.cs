using UnityEngine;

public class SpriteStyler : MonoBehaviour
{
    public void Tint(SpriteRenderer renderer, Color color)
    {
        renderer.color = color;
    }
}
