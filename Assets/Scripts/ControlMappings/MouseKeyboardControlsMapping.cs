using UnityEngine;

public class MouseKeyboardControlsMapping : MonoBehaviour
{
    public static MouseKeyboardControlsMapping Instance { get; private set; }

    // Mappings
    // General / multi-use
    public static KeyCode CANCEL_GENERAL = KeyCode.Escape;

    // Player movement
    public static KeyCode MOVE_PLAYER_LEFT = KeyCode.A;
    public static KeyCode MOVE_PLAYER_RIGHT = KeyCode.D;
    public static KeyCode MOVE_PLAYER_UP = KeyCode.W;
    public static KeyCode MOVE_PLAYER_DOWN = KeyCode.S;

    // Pickup Item
    public static KeyCode PICKUP_ITEM = KeyCode.E;

    // Interactions
    public static KeyCode INTERACT = KeyCode.F;

    // Menus
    public static KeyCode TOGGLE_INVENTORY_MENU = KeyCode.I;

    // Build mode
    public static KeyCode TOGGLE_BUILD_MODE = KeyCode.B;
    public static KeyCode ROTATE_LEFT = KeyCode.LeftBracket;
    public static KeyCode ROTATE_RIGHT = KeyCode.RightBracket;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
