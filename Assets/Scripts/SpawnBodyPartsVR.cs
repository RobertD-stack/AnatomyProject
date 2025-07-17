using UnityEngine;
using System.Collections.Generic;

// DEPRECATED 1.16.5 6/17
public class SpawnBodyPartsVR : MonoBehaviour
{
    public GameObject[] bodyParts;
    public BoxCollider[] spawnColliders;
    public GameObject[] spawnObjects;
    public Material defaultMaterial;
    public List<GameObject> displacedParts;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<int> listNumbers = new List<int>();

        int number;

        // Generate a list of available spawn indices and shuffle it
        List<int> availableSpawnIndices = new List<int>();
        for (int i = 0; i < spawnObjects.Length; i++)
        {
            availableSpawnIndices.Add(i);
        }

        // Shuffle the list
        for (int i = 0; i < availableSpawnIndices.Count; i++)
        {
            int temp = availableSpawnIndices[i];
            int randomIndex = Random.Range(i, availableSpawnIndices.Count);
            availableSpawnIndices[i] = availableSpawnIndices[randomIndex];
            availableSpawnIndices[randomIndex] = temp;
        }

        if (bodyParts.Length == 0) return;

        for (int i = 0; i < bodyParts.Length; i++)
        {


            Vector3 spawnPos = spawnObjects[availableSpawnIndices[i]].transform.position;

            //Create temporary object
            GameObject temp = Instantiate(bodyParts[i], spawnPos, bodyParts[i].transform.rotation);
            temp.GetComponent<Renderer>().material = defaultMaterial;
            temp.AddComponent<Draggable>(); // Add Draggable Property
            temp.AddComponent<Matched>(); // Add Matched Property


            Highlight label = temp.AddComponent<Highlight>(); // Add Label
            label.label = bodyParts[i].name;

            temp.AddComponent<BoxCollider>(); // Add Box Collider
            SnapToPlace snapComponent = temp.AddComponent<SnapToPlace>(); // Add Snap to Place Component Onto Spawned Objects
            snapComponent.slotObject = bodyParts[i]; // Add the slot object as an object to snap to
            displacedParts.Add(temp); // This list is accessed by JoinParts.cs




        }
        // Disable all colliders
        for (int i = 0; i < spawnColliders.Length; i++)
        {
            spawnColliders[i].enabled = false;
        }

    }



}
