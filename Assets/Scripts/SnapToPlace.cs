using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
/* *** SUMMARY *** 

*/

public class SnapToPlace : MonoBehaviour
{
    public bool matched = false;
    public GameObject slotObject;

    // public InputActionReference triggerAction;


    void Update()
    {
        // For keyboard and mouse
        if (Input.GetMouseButtonUp(0))
        {
            Snap();

        }

        // // VR

        // float triggerValue = triggerAction.action.ReadValue<float>();

        // if (triggerValue < 0.5f) // Example threshold
        // {
        //     Debug.Log("Trigger released");
        //     Snap();
        // }
    }



    void Snap()
    {
        Vector3 closestPos = transform.position;
        float minDistance = float.MaxValue; // Start at max value and work down

        float distance = Vector3.Distance(transform.position, slotObject.transform.position); // Get Distance Between Object and Draggable

        if (distance < minDistance && distance < 2f)
        {
            minDistance = distance;
            closestPos = slotObject.transform.position;
            transform.position = closestPos;

            if (matched)
            {
                return;
            }
            matched = true;
            slotObject.GetComponent<Matched>().setMatched(matched);
            gameObject.GetComponent<Matched>().setMatched(matched);
            gameObject.GetComponent<Highlight>().ToggleMaterial(false);
        }
    }

}