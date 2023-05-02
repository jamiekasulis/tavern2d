using UnityEngine;

/**
 * Defines a grid area where Build Mode can happen.
 * In this area, objects which are PlaceableObjects can be moved around,
 * rotated, added, or removed.
 * 
 * When Build Mode is on, it will use the bounds of the BuildableGridArea to 
 * highlight the build area for the user to see, and also it will use the bounds
 * to determine the validity of all placements.
 */
[RequireComponent(typeof(BuildMode))]
public class BuildableGridArea : GridArea
{
    private Grid buildModeGrid;

    public override void Awake()
    {
        base.Awake();
        buildModeGrid = gameObject.GetComponentInParent<Grid>();
        if (buildModeGrid == null)
        {
            throw new System.Exception("Could not find build mode grid for BuildableGridArea!");
        }
        SnapPositionToGrid();
    }

    private void SnapPositionToGrid()
    {
        Vector3 pos = new(
            Mathf.RoundToInt(gameObject.transform.position.x),
            Mathf.RoundToInt(gameObject.transform.position.y),
            0
        );
        gameObject.transform.SetPositionAndRotation(pos, Quaternion.identity);
    }

    public override void Update()
    {
        base.Update();
    }
}
