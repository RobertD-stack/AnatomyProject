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
    public GameObject globalVariables;
    public BodyPartGroupType groupType = BodyPartGroupType.Skeleton;
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



        foreach (Transform child in transform)
        {
            if (child.childCount > 0)
            {
                foreach (Transform part in child)
                    RegisterSlot(part, allItems);
            }
            else
            {
                RegisterSlot(child, allItems);
            }
        }

        if (createNewDescriptionItems == CreateNewDescriptionItems.True)
        {
            string json = JsonUtility.ToJson(new Items { bodyParts = allItems }, true);
            System.IO.File.WriteAllText(path, json);
            Debug.Log($"Saved {allItems.Count} items to {path}");
        }


    }

    void RegisterSlot(Transform part, List<Item> allItems)
    {
        part.gameObject.tag = "Slots";
        if (part.GetComponent<Matched>() == null)
            part.gameObject.AddComponent<Matched>();

        slots.Add(part);

        if (createNewDescriptionItems != CreateNewDescriptionItems.True)
            return;

        Item tempItem = new Item { name = part.name, description = "" };
        if (!allItems.Exists(item => item.name == tempItem.name))
            allItems.Add(tempItem);
    }
}
