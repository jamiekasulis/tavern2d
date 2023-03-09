using UnityEngine;

public class MouseUtils : MonoBehaviour
{
    public static Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
