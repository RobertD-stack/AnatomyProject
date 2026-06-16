using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* *** SUMMARY *** 
Assign slot objects(item assets acting as slots to drag spawned items into) necessary components

Also overwrite descriptions.json with new descriptions
*/

public enum CreateNewDescriptionItems
{
    True,
    False
}


public class AssignSlotObject : MonoBehaviour
{
    public List<Transform> slots = new List<Transform>();
    public CreateNewDescriptionItems createNewDescriptionItems;
    void Awake() {

        // Read itemDescriptions data and add it to a dictionary 
        // TODO: Make this more effiicent
        string path = "Assets/Resources/Edited Human Parts/itemDescriptions.json";
        List<Item> allItems = new List<Item>();

        if (System.IO.File.Exists(path))
        {
            string existingJson = System.IO.File.ReadAllText(path);

            Items itemList = JsonUtility.FromJson<Items>(existingJson);

            // Merge existing data
            allItems.AddRange(itemList.bodyParts); 
        }



        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.tag = "Slots";
            child.gameObject.AddComponent<Matched>();

            slots.Add(child);

            if (createNewDescriptionItems == CreateNewDescriptionItems.True)
            {
                Item tempItem = new Item
                {
                    name = child.name,
                    description = ""
                };

                // Only add if an item with the same name doesn't already exist
                bool alreadyExists = allItems.Exists(item => item.name == tempItem.name);

                if (!alreadyExists)
                {
                    allItems.Add(tempItem);
                }
            }
        }

        if (createNewDescriptionItems == CreateNewDescriptionItems.True)
        {
            string json = JsonUtility.ToJson(new Items { bodyParts = allItems }, true);
            System.IO.File.WriteAllText(path, json);
            Debug.Log($"Saved {allItems.Count} items to {path}");
        }


    }
}

