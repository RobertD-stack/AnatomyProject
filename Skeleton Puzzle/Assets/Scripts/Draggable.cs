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

    void OnMouseDown()
    {
        isDragging = true;

        // Calculate offset between object position and mouse position at time of click
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        offset = transform.position - mouseWorldPos;
    }

    // Initiate drag
    void StartDragging()
    {
        // Check if we're supposed to be dragging
        if (isDragging)
        {
            // Debug.Log("Dragging");

            Vector3 newPos = GetMouseWorldPosition() + offset; // Apply offset to maintain relative grab position
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
