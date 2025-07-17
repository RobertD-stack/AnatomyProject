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

    void Start()
    {
        isDragging = true;
        draggable = true;
    }

    // When the object is spawned in, it should be already dragged as long as isDragging is true by default
    void Update()
    {
        if (Input.GetMouseButton(0) && draggable)
        {
            StartDragging();

        }
        // No longer dragging
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
    }

    // Initiate drag
    void StartDragging()
    {
        // Check if we're supposed to be dragging
        if (isDragging)
        {
            // Debug.Log("Dragging");

            Vector3 newPos = GetMouseWorldPosition();
            newPos.z = transform.position.z; // Lock the Z value to original
            transform.position = newPos;
        }

    }

    // Get position of mouse in world
    Vector3 GetMouseWorldPosition()
    {
        Vector3 screenMousePos = Input.mousePosition;
        screenMousePos.z = Camera.main.WorldToScreenPoint(transform.position).z; // get current object's z-distance from camera
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }

}
