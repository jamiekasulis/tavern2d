using UnityEngine;

public class MouseKeyboardControlsMapping : MonoBehaviour
{
    public static MouseKeyboardControlsMapping Instance { get; private set; }

    // Mappings
    // General / multi-use
    public static KeyCode CANCEL_GENERAL = KeyCode.Escape;


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
