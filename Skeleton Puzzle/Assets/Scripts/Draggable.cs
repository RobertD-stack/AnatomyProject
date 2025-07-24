using UnityEngine;
/* *** SUMMARY *** 

*/

public class Draggable : MonoBehaviour
{
    // True by default
    public bool draggable;

    // Offset from mouse
    private Vector3 offset;

    // Is dragging is set to true by default to initiate drag as soon as the item is spawned
    public bool isDragging;

    public GameObject globalVariable;

    void Start()
    {
        globalVariable = GameObject.FindGameObjectWithTag("GlobalVariables");

        isDragging = true;
        draggable = true;
    }

    // When the object is spawned in, it should be already dragged as long as isDragging is true by default
    void Update()
    {
        if (Input.GetMouseButton(0) && draggable)
        {
            StartDragging();
            globalVariable.GetComponent<IsDragging>().isDragging = true;

        }

        // No longer dragging
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            globalVariable.GetComponent<IsDragging>().isDragging = false;
        }
    }

    // Takes over after the object was already spawned in
    void OnMouseDown()
    {
        isDragging = true;

        StartDragging();
    }

    // Initiate drag
    void StartDragging()
    {
        // Check if we're supposed to be dragging
        if (isDragging)
        {
            // Debug.Log("Dragging");

            Vector3 newPos = GetMouseWorldPosition() + offset;
            newPos.z = transform.position.z;

            // Add the difference between center and pivot
            Vector3 centerOffset = GetVisualCenterOffset();
            centerOffset.z = 0f;

            transform.position = newPos - centerOffset;
        }
    }

    // Get position of mouse in world
    Vector3 GetMouseWorldPosition()
    {
        Vector3 screenMousePos = Input.mousePosition;
        screenMousePos.z = Camera.main.WorldToScreenPoint(transform.position).z; // get current object's z-distance from camera
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }


    // Get the visual center of the object
    Vector3 GetVisualCenterOffset()
    {
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Vector3 center = renderer.bounds.center;
            Vector3 pivot = transform.position;
            return center - pivot;
        }
        return Vector3.zero;
    }
}
