using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* *** SUMMARY *** 
Assign final objects(item assets that are in the correct final location) necessary components
*/

public class AssignFinalObject : MonoBehaviour
{
    public List<Transform> final = new List<Transform>();
    void Awake() {
        foreach (Transform child in gameObject.transform) {
            foreach (Transform grandchild in child) {
                grandchild.gameObject.GetComponent<MeshRenderer>().enabled = false;
                child.gameObject.tag = "Final";
                child.gameObject.AddComponent<Highlight>();
                child.gameObject.AddComponent<Matched>();
                final.Add(child);
            }

        }
    }
}
