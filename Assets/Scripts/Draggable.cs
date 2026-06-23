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

    public Camera mainCamera;

    public float distanceFromCamera = 5f;
    public float scrollSpeed = 2f;
    public float minDistance = 2f;
    public float maxDistance = 20f;

    void Start()
    {
        mainCamera = Camera.main;
        globalVariable = GameObject.FindGameObjectWithTag("GlobalVariables");

        isDragging = false;
        draggable = true;
    }

    // Update only continues an active drag started via OnMouseDown on this object.
    void Update()
    {

        if (!isDragging || !draggable)
            return;

        if (Input.GetMouseButton(0))
        {
            StartDragging();
            globalVariable.GetComponent<IsDragging>().isDragging = true;
        }
        else
        {
            isDragging = false;
            globalVariable.GetComponent<IsDragging>().isDragging = false;
        }
    }

    // Takes over after the object was already spawned in
    void OnMouseDown()
    {
        isDragging = true;

        Vector3 toObject = transform.position - mainCamera.transform.position;
        distanceFromCamera = Vector3.Dot(toObject, mainCamera.transform.forward);
        distanceFromCamera = Mathf.Clamp(distanceFromCamera, minDistance, maxDistance);

        StartDragging();
    }

    // Initiate drag
    void StartDragging()
    {
        // Check if we're supposed to be dragging
        if (isDragging)
        {
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                distanceFromCamera += scroll * scrollSpeed;
                distanceFromCamera = Mathf.Clamp(distanceFromCamera, minDistance, maxDistance);
            }

            Vector3 newPos = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;
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
