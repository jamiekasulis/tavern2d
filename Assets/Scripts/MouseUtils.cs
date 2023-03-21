using UnityEngine;
using UnityEngine.UIElements;

public class MouseUtils : MonoBehaviour
{
    public static bool IsLeftClickOnly(MouseDownEvent evt)
    {
        // Do not use evt.shiftKey as from my testing this is inaccurate 1/2 the time
        return evt.button == 0 && !Input.GetKey(KeyCode.LeftShift);
    }

    public static bool IsShiftLeftClick(MouseDownEvent evt)
    {
        // Do not use evt.shiftKey as from my testing this is inaccurate 1/2 the time
        return evt.button == 0 && Input.GetKey(KeyCode.LeftShift);
    }

    public static bool IsRightClick(MouseDownEvent evt)
    {
        return evt.button == 1;
    }

    public static Vector2 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
