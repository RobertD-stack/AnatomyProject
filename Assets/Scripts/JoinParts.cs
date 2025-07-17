using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
/* *** SUMMARY *** 

*/

public class JoinParts : MonoBehaviour
{

    public GameObject globalVariables;
    // FIXME: Get Objects By Tag Instead
    private GameObject[] slotParts; // Array of slotParts

    // FIXME: Get Objects By Tag Instead
    private GameObject[] finalParts; // Array of parts already in their final positions

    //  FIXME: Add objects by tag
    private GameObject[] spawnedItems;

    public float smooth = 1f;

    public List<GameObject> slotList = new List<GameObject>();
    public List<GameObject> finalList = new List<GameObject>();
    public List<GameObject> spawnList = new List<GameObject>();


    void Start()
    {
        globalVariables = GameObject.FindWithTag("GlobalVariables"); ;
        // All the separate arrays(spawnedItems, slotParts, and finalParts) have to be sorted alphabetically in order to ensure
        // that they are being organized correctly
        slotParts = GameObject.FindGameObjectsWithTag("Slots");
        finalParts = GameObject.FindGameObjectsWithTag("Final");


        slotList.AddRange(slotParts);
        slotList.Sort((a, b) => a.name.CompareTo(b.name));

        finalList.AddRange(finalParts);
        finalList.Sort((a, b) => a.name.CompareTo(b.name));


    }

    void Update() // Constantly check if all objects are matched
    {
        // Constantly update list of spawned items
        spawnedItems = GameObject.FindGameObjectsWithTag("SpawnedItem");

        
        for (int i = 0; i < slotParts.Length; i++) // Iterate through all the displaced parts
        {
            // Debug.Log("Checking");
            if (slotParts[i].GetComponent<Matched>().matched == false) // If >=1 displaced part is not matched, do not move parts
            {
                // Debug.Log("Found false");
                return;
            }
        }

        // Set the global variable that all items are matched to true
        globalVariables.GetComponent<AllMatched>().setAllMatched(true);

        foreach (GameObject part in spawnedItems)
        {
            if (part.GetComponent<Matched>().matched == false)
            {
                Destroy(part);
            }
        }

        // Disable All Mesh Renderers for slots
        foreach (GameObject slot in slotList)
        {
            slot.GetComponent<MeshRenderer>().enabled = false;
        }
        moveToPosition(); // If all displaced parts are matched, move each part




    }



    void moveToPosition() // Move all parts to final position
    {

        // Sort the list of spawned items to match the other lists
        spawnList.Clear();
        spawnList.AddRange(spawnedItems);
        spawnList.Sort((a, b) => a.name.CompareTo(b.name));        

        bool allInPlace = false;
        for (int i = 0; i < slotList.Count; i++) // Iterate through all the displaced parts
        {
            // We check if the object is within the range of an acceptable 
            bool isClose = Vector3.Distance(spawnList[i].transform.position, finalList[i].transform.position) < 0.01f;
            bool isRotated = Quaternion.Angle(spawnList[i].transform.rotation, finalList[i].transform.rotation) < 1f;


            // TODO: Allow Items to be rotated
            if (isClose && isRotated)
            {
                Debug.Log("Finished moving no longer locked");
                globalVariables.GetComponent<MoveItems>().setMoveItems(false);
                return;
            }
            if (!globalVariables.GetComponent<MoveItems>().moveItems)
            {
                // Debug.Log("Finished moving no longer locked");
                return;
            }
            Debug.Log("Moving");
            //Position
            Vector3 designatedPosition = new Vector3(finalList[i].transform.position.x, finalList[i].transform.position.y, finalList[i].transform.position.z); // Set the designated position to the final part's position
            // Move Slot Part
            Transform slotTransform = slotList[i].transform; // Set the current transform as the current slot part in the array
            slotTransform.position = Vector3.Lerp(slotTransform.position, designatedPosition, smooth * Time.deltaTime); // Move each slot to designated position

            // Move SpawnedItems
            Transform displacedTransform = spawnList[i].transform; // Set the current transform as the current displaced part in the array
            displacedTransform.position = Vector3.Lerp(displacedTransform.position, designatedPosition, smooth * Time.deltaTime); // Move each part to designated position



            // Change Rotations
            Quaternion designatedRotation = finalList[i].transform.rotation; // Get full rotation from final part
            slotTransform.rotation = Quaternion.Lerp(slotTransform.rotation, designatedRotation, smooth * Time.deltaTime); // Smoothly rotate slot
            displacedTransform.rotation = Quaternion.Lerp(displacedTransform.rotation, designatedRotation, smooth * Time.deltaTime); // Smoothly rotate displaced


            //Set Draggable property for all displaced to false
            spawnList[i].GetComponent<Draggable>().draggable = false;

            spawnList[i].GetComponent<Highlight>().ToggleMaterial(false);


        }

    }
    




}


