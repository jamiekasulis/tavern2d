using UnityEngine;

public class MouseKeyboardControlsMapping : MonoBehaviour
{
    public static MouseKeyboardControlsMapping Instance { get; private set; }

    // Mappings
    // General / multi-use
    public static KeyCode CANCEL_GENERAL = KeyCode.Escape;

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
