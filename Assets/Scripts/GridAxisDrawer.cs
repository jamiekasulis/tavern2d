using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Grid))]
public class GridAxisDrawer : MonoBehaviour
{
    private Grid grid;
    [SerializeField] public Color color = Color.cyan;

    private void Awake()
    {
        grid = gameObject.GetComponent<Grid>();
        Debug.DrawLine(new(grid.transform.position.x - 100, 0, 1), new(grid.transform.position.x + 100, 0, 1), color, 1000);
        Debug.DrawLine(new(0, grid.transform.position.y - 100, 1), new(0, grid.transform.position.y + 100, 1), color, 1000);
    }
}
