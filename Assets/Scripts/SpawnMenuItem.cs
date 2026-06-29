using UnityEngine;

using System.Collections;

using System.Collections.Generic;

using UnityEngine.InputSystem;

using UnityEngine.InputSystem.XR;

using UnityEngine.UI;

using System;



using UnityEngine.XR.Interaction.Toolkit.Transformers;





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

    public GameObject helperPrefab;

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
        // If the mode is Mouse and Keyboard, we don't need the button
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

        ToggleVR toggleVR = globalVariables.GetComponent<ToggleVR>();

        if (toggleVR != null && toggleVR.VRToggle == VR.On)

            return;



        if (globalVariables.GetComponent<IsDragging>().isDragging == false)

        {

            spawnItem();



        }





    }



    public void spawnItem()

    {

        Debug.Log("Spawning " + menuItemName);



        if (spawnParent == null)

        {

            Debug.LogError("SpawnMenuItem: spawnParent is not assigned.");

            return;

        }



        Transform spawnTransform = spawnParent.transform;



        try

        {

            GameObject temp = Instantiate(menuItem);

            // GameObject helper = Instantiate(helperPrefab);  // Helper is a small cube that helps with the interacting
            // helper.transform.SetParent(temp.transform, false);
            // helper.transform.localPosition = Vector3.zero;
            // helper.transform.localRotation = Quaternion.identity;
            // helper.transform.localScale = Vector3.one;
            float objectScale = globalVariables.GetComponent<itemSize>().objectScale;

            temp.transform.localScale = Vector3.one * objectScale;



            temp.AddComponent<Draggable>();

            temp.AddComponent<Matched>();

            temp.AddComponent<DeleteItem>();

            if (temp.GetComponent<Highlight>() == null)

                temp.AddComponent<Highlight>();

            if (temp.GetComponent<BoxCollider>() == null)

                temp.AddComponent<BoxCollider>();



            Rigidbody rb = temp.GetComponent<Rigidbody>();

            if (rb == null)

                rb = temp.AddComponent<Rigidbody>();

            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;



            if (temp.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>() == null)

                temp.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

            if (temp.GetComponent<XRGeneralGrabTransformer>() == null)

                temp.AddComponent<XRGeneralGrabTransformer>();

            // Allow the item to be grabbed and dropped dynamically

            temp.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().useDynamicAttach = true;

            Highlight label = temp.GetComponent<Highlight>();

            label.label = menuItemName;



            SnapToPlace slotComponent = temp.AddComponent<SnapToPlace>();

            slotComponent.slotObject = slot;



            temp.tag = "SpawnedItem";



            PlaceAtSpawnParent(temp.transform, spawnTransform);

            temp.transform.SetParent(spawnTransform, true);




            if (rb != null)

            {

                rb.position = temp.transform.position;

                rb.rotation = temp.transform.rotation;

            }

        }

        catch (Exception)

        {

            Debug.Log("The item for this menu item has not been assigned");

        }

    }



    static void PlaceAtSpawnParent(Transform spawned, Transform spawnParentTransform)

    {

        // spawned.SetPositionAndRotation(spawnParentTransform.position, spawnParentTransform.rotation);
        spawned.position = spawnParentTransform.position;


        // Many skeleton addressables have their mesh offset from the root pivot.

        Renderer renderer = spawned.GetComponentInChildren<Renderer>();

        if (renderer == null)

            return;



        Vector3 meshCenterOffset = renderer.bounds.center - spawned.position;

        spawned.position = spawnParentTransform.position - meshCenterOffset;

    }

}


