using UnityEngine;

public class MouseKeyboardControlsMapping : MonoBehaviour
{
    public static MouseKeyboardControlsMapping Instance { get; private set; }

    // Mappings

    // Player movement
    public static KeyCode MOVE_PLAYER_LEFT = KeyCode.A;
    public static KeyCode MOVE_PLAYER_RIGHT = KeyCode.D;
    public static KeyCode MOVE_PLAYER_UP = KeyCode.W;
    public static KeyCode MOVE_PLAYER_DOWN = KeyCode.S;

    // Interaction
    public static KeyCode PICKUP_ITEM = KeyCode.E;

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
