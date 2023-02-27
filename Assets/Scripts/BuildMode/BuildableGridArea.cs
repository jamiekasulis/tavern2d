using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class BuildableGridArea : GridArea
{
    [SerializeField] private Tilemap BuildableAreaTilemap;
    [SerializeField] private Sprite BuildableSprite;

    private void UpdateBuildableCells()
    {
        TileBase[] buildableAreaTiles = GetTiles(BuildableAreaTilemap, false)
            .Where(tb => tb.name == BuildableSprite.name)
            .ToArray();

        // Mark all the grid cells within buildableAreaTiles as buildable = true.
    }

    public override void Update()
    {
        base.Update();
        UpdateBuildableCells();
    }
}
