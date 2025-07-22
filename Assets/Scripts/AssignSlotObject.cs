using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* *** SUMMARY *** 
Assign slot objects(item assets acting as slots to drag spawned items into) necessary components

Also overwrite descriptions.json with new descriptions
*/


public class AssignSlotObject : MonoBehaviour
{
    public List<Transform> slots = new List<Transform>();
    void Awake() {

        // Read itemDescriptions data and add it to a dictionary 
        // TODO: Make this more effiicent
        string path = "Assets/Resources/Edited Human Parts/itemDescriptions.json";
        List<Item> allItems = new List<Item>();


        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.tag = "Slots";
            child.gameObject.AddComponent<Matched>();

            slots.Add(child);

            // if (System.IO.File.Exists(path))
            // {
            //     Item tempItem = new Item {
            //         name=child.name,
            //         description=""
            //     };

            //     allItems.Add(tempItem);
            //     // Optional: Save to JSON


            // }
            // else
            // {
            //     Debug.Log(path + " is not a valid path!");
            // }
            // string json = JsonUtility.ToJson(new ItemListWrapper { items = allItems }, true);
            // System.IO.File.WriteAllText(path, json);
            // Debug.Log($"Saved {allItems.Count} items to {path}");



            
            
        }

    }
}

[System.Serializable]
public class ItemListWrapper
{
    public List<Item> items;
}
