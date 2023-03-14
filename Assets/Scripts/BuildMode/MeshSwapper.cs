using UnityEngine;

/**
 * Responsible for replacing the "mesh" (which actually refers to a game object with
 * a sprite and collider) according to rotations.
 */
[ExecuteInEditMode]
public class MeshSwapper : MonoBehaviour
{
    [SerializeField] public Mesh2D Front, Back, Left, Right, Default;
    public Mesh2D Current { get; private set; }

    private void Awake()
    {
        Current = Instantiate(Default, new Vector3(0, 0, 1), Quaternion.identity, gameObject.transform);
        Current.transform.localPosition = Vector3.zero;
    }

    public void LoadMeshForDirection(DirectionEnum newMeshDir, Vector3 position)
    {
        Mesh2D meshToInstantiate;
        if (newMeshDir == DirectionEnum.front)
        {
            meshToInstantiate = Front;
        }
        else if (newMeshDir == DirectionEnum.back)
        {
            meshToInstantiate = Back;
        }
        else if (newMeshDir == DirectionEnum.left)
        {
            meshToInstantiate = Left;
        }
        else
        {
            meshToInstantiate = Right;
        }

        Destroy(Current.gameObject, 0);
        Current = Instantiate(meshToInstantiate, position, Quaternion.identity, gameObject.transform);
        Current.transform.localPosition = Vector3.zero;
    }
}

public enum DirectionEnum
{
    front = 0,
    left = 1,
    back = 2,
    right = 3
}
