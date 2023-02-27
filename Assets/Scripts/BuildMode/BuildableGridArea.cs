using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class BuildableGridArea : GridArea
{
    [SerializeField] private Tilemap BuildableAreaTilemap;
    [SerializeField] private Sprite BuildableAreaSprite, PlacementOKSprite, PlacementBadSprite;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Update()
    {
        base.Update();
    }
}
