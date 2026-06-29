using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/* *** SUMMARY *** 

*/

public class SnapToPlace : MonoBehaviour
{
    public bool matched = false;
    public GameObject slotObject;

    public float snapDistance = 2f;

    XRGrabInteractable grab;

    void OnEnable()
    {
        grab = GetComponent<XRGrabInteractable>();
        if (grab != null)
            grab.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        if (grab != null)
            grab.selectExited.RemoveListener(OnReleased);
    }

    void Update()
    {

        if (Input.GetMouseButtonUp(0))
            Snap();
    }

    void OnReleased(SelectExitEventArgs args)
    {
        Snap();
    }

    void Snap()
    {
        if (slotObject == null || matched)
            return;
        // We calculated the distance between the renderers
        Renderer itemRenderer = GetComponentInChildren<Renderer>();
        Renderer slotRenderer = slotObject.GetComponentInChildren<Renderer>();

        Vector3 itemCenter = itemRenderer != null ? itemRenderer.bounds.center : transform.position;
        Vector3 slotCenter = slotRenderer != null ? slotRenderer.bounds.center : slotObject.transform.position;
        float threshold = GetSnapDistance(itemRenderer, slotRenderer);

        if (Vector3.Distance(itemCenter, slotCenter) > threshold)
        {
            Matched matchedComponent = GetComponent<Matched>();
            if (matchedComponent != null)
                matchedComponent.setMatched(false);
            return;
        }

        Rigidbody rb = GetComponent<Rigidbody>();

        transform.SetParent(null, true);
        transform.SetPositionAndRotation(slotObject.transform.position, slotObject.transform.rotation);
        transform.localScale = slotObject.transform.lossyScale;

        itemRenderer = GetComponentInChildren<Renderer>();
        if (itemRenderer != null)
        {
            slotRenderer = slotObject.GetComponentInChildren<Renderer>();
            slotCenter = slotRenderer != null ? slotRenderer.bounds.center : slotObject.transform.position;
            Vector3 centerOffset = itemRenderer.bounds.center - transform.position;
            transform.position = slotCenter - centerOffset;
        }

        if (rb != null)
        {
            rb.position = transform.position;
            rb.rotation = transform.rotation;
        }

        Destroy(GetComponent<Draggable>());

        matched = true;

        Matched slotMatched = slotObject.GetComponent<Matched>();
        if (slotMatched != null)
            slotMatched.setMatched(true);

        Matched itemMatched = GetComponent<Matched>();
        if (itemMatched != null)
            itemMatched.setMatched(true);

        // Disable the slot object renderer
        slotObject.GetComponent<MeshRenderer>().enabled = false;
    }

    float GetSnapDistance(Renderer itemRenderer, Renderer slotRenderer)
    {
        float itemExtent = itemRenderer != null ? itemRenderer.bounds.extents.magnitude : 1f;
        float slotExtent = slotRenderer != null ? slotRenderer.bounds.extents.magnitude : 1f;
        return Mathf.Max(snapDistance, Mathf.Max(itemExtent, slotExtent) * 1.25f);
    }
}
