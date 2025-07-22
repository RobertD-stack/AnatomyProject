using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using System;

/* *** SUMMARY *** 
This script is placed on every single UI Menu Item and is used to spawn the corresponding addressable asset while also placing necessary components
*/

public enum Mode
{
    MouseAndKeyboard,
    VR
}

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

    public Mode mode;




    void Start()
    {
        if (mode == Mode.MouseAndKeyboard)
        {
            gameObject.GetComponent<Button>().enabled = false;
        }
        globalVariables = GameObject.FindGameObjectWithTag("GlobalVariables");
        snapArr = GameObject.FindGameObjectsWithTag("Slots");
        foreach (GameObject slotObject in snapArr)
        {
            slotDictionary.Add(slotObject.name.ToLower(), slotObject);
        }

        try
        {
            slot = slotDictionary[gameObject.name.ToLower()];
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message); // or just Debug.Log(e) for full exception info
        }



        gameObject.GetComponent<Button>().onClick.AddListener(spawnItem);

    }

    void OnMouseDown()
    {

        if (globalVariables.GetComponent<IsDragging>().isDragging == false)
        {
            spawnItem();

        }


    }

    void spawnItem()
    {
        Debug.Log("Spawning " + menuItemName);
        // Must zero out z and set to 1.89
        Vector3 spawnPos = gameObject.transform.position;
        spawnPos.z = 3.47f;
        try
        {
            GameObject temp = Instantiate(menuItem, spawnPos, menuItem.transform.rotation);
            float objectScale = globalVariables.GetComponent<itemSize>().objectScale;
            temp.transform.localScale = new Vector3(objectScale, objectScale, objectScale);
            temp.transform.SetParent(spawnParent.transform);
            temp.AddComponent<Draggable>(); // Add Draggable Property
            temp.AddComponent<Matched>(); // Add Matched Property
            temp.AddComponent<DeleteItem>(); // Allow item to be deleted
            if (temp.GetComponent<Highlight>() == null)
            {
                temp.AddComponent<Highlight>();
            }
            if (temp.GetComponent<BoxCollider>() == null)
            {
                temp.AddComponent<BoxCollider>();
            }
            // Highlight label = temp.AddComponent<Highlight>(); // Add Label
            Highlight label = temp.GetComponent<Highlight>(); // Get Label
            label.label = menuItemName;

            SnapToPlace slotComponent = temp.AddComponent<SnapToPlace>(); // Add Snap to Place Component Onto Spawned Objects
            slotComponent.slotObject = slot;

            temp.tag = "SpawnedItem";

        }
        catch (Exception ex)
        {
            Debug.Log("The item for this menu item has not been assigned");
        }









    }
}
