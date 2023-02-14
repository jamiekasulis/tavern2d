using UnityEngine;

public class MouseKeyboardControlsMapping : MonoBehaviour
{
    public static MouseKeyboardControlsMapping Instance { get; private set; }

    // Mappings
    public static KeyCode MOVE_PLAYER_LEFT = KeyCode.A;
    public static KeyCode MOVE_PLAYER_RIGHT = KeyCode.D;
    public static KeyCode MOVE_PLAYER_UP = KeyCode.W;
    public static KeyCode MOVE_PLAYER_DOWN = KeyCode.S;

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
