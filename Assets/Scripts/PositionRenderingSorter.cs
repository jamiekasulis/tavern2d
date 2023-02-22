using UnityEngine;

/**
 * Dynamically adjusts the sorting layer of the object's renderer according to
 * the object's Y position in world space. This enables objects being correctly
 * positioned in front/behind other objects.
 */
public class PositionRenderingSorter : MonoBehaviour
{
    [SerializeField] private int sortingOrderBase = 5000;
    [SerializeField] private int offset = -2; // Will be set to equal offsetOverride if it is non-null.

    /* Set this to true for static/unmoving objects so save on performance. These
     * objects will only ever need to be sorted once anyways.
     * Leave this to true for any objects which can move around.
     */
    [SerializeField] private bool runOnlyOnce = true;

    private Renderer renderer;

    private void Awake()
    {
        renderer = gameObject.GetComponent<Renderer>();
    }

    private void LateUpdate()
    {
        // Do a late update so that transform.position is guaranteed to already
        // reflect the latest changes
        renderer.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);

        if (runOnlyOnce)
        {
            Destroy(this);
        }
    }
}
