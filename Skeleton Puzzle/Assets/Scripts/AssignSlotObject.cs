using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* *** SUMMARY *** 
Assign slot objects(item assets acting as slots to drag spawned items into) necessary components
*/


public class AssignSlotObject : MonoBehaviour
{
    public List<Transform> slots = new List<Transform>();
    void Awake() {
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.tag = "Slots";
            child.gameObject.AddComponent<Matched>();

            slots.Add(child);
            
        }

    }
}
