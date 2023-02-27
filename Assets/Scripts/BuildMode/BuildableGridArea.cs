using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

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
