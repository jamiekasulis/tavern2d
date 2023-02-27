using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    public CompositeCollider2D collider { get; private set; }

    private void Awake()
    {
        collider = gameObject.GetComponent<CompositeCollider2D>();
    }

}
