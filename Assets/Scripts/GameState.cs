using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;
    [SerializeField] public Player Player;

    private void Awake()
    {
        if (Instance != null && this != Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
}
