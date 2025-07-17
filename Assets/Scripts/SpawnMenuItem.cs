using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
/* *** SUMMARY *** 

*/

public class SpawnMenuItem : MonoBehaviour
{
    public string menuItemName;
    public GameObject menuItem;

    // From the slots object
    public GameObject slot;
    public Material defaultMaterial;
    public GameObject spawnParent;

    // Dictionary helps search for the slot object for the snapToPlace script
    public Dictionary<string, GameObject> slotDictionary = new Dictionary<string, GameObject>();
    public GameObject[] snapArr;

    public GameObject globalVariables;




    void Start()
    {
        globalVariables = GameObject.FindGameObjectWithTag("GlobalVariables");
        snapArr = GameObject.FindGameObjectsWithTag("Slots");
        foreach (GameObject slotObject in snapArr)
        {
            slotDictionary.Add(slotObject.name.ToLower(), slotObject);
        }

        slot = slotDictionary[gameObject.name.ToLower()];


        gameObject.GetComponent<Button>().onClick.AddListener(spawnItem);

    }

    void OnMouseDown()
    {

        spawnItem();

    }

    void spawnItem()
    {
        Debug.Log("Spawning " + menuItemName);
        // Must zero out z and set to 1.89
        Vector3 spawnPos = gameObject.transform.position;
        spawnPos.z = 3.47f;
        GameObject temp = Instantiate(menuItem, spawnPos, menuItem.transform.rotation);
        temp.transform.localScale = temp.transform.localScale * globalVariables.GetComponent<itemSize>().objectScale;
        temp.transform.SetParent(spawnParent.transform);
        temp.AddComponent<Draggable>(); // Add Draggable Property
        temp.AddComponent<Matched>(); // Add Matched Property
        temp.AddComponent<DeleteItem>(); // Allow item to be deleted



        // Highlight label = temp.AddComponent<Highlight>(); // Add Label
        Highlight label = temp.GetComponent<Highlight>(); // Get Label
        label.label = menuItemName;




        SnapToPlace slotComponent = temp.AddComponent<SnapToPlace>(); // Add Snap to Place Component Onto Spawned Objects
        slotComponent.slotObject = slot;

        temp.tag = "SpawnedItem";


    }
}
