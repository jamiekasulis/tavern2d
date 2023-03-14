using UnityEngine;

/**
 * Responsible for replacing the "mesh" (which actually refers to a game object with
 * a sprite and collider) according to rotations.
 */
public class MeshSwapper : MonoBehaviour
{
    [SerializeField] public Mesh2D Front, Back, Left, Right, Default;

    public Mesh2D GetMeshForDirection(DirectionEnum newMeshDir)
    {
        if (newMeshDir == DirectionEnum.front)
        {
            return Front;
        }
        else if (newMeshDir == DirectionEnum.back)
        {
            return Back;
        }
        else if (newMeshDir == DirectionEnum.left)
        {
            return Left;
        }
        else
        {
            return Right;
        }
    }
}

public enum DirectionEnum
{
    front = 0,
    left = 1,
    back = 2,
    right = 3
}
