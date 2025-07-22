using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;


/* *** SUMMARY *** 
    This script handles creating menu items for each slot item.
*/

public enum ImageLoadMethod
{
    FilePath,
    Dictionary
}

public enum AssetLoadMethod
{
    FilePath, 
    List
}

public class ManageMenuItems : MonoBehaviour
{
    public GameObject menuItem;
    public GameObject[] slotObjects; // <-- This allows us to determine how many menu items to spawn in
    public List<GameObject> parts; // List of parts

    public List<GameObject> menuItemList = new List<GameObject>(); // List of menu items

    public GameObject inputField; // Search field
    public string searchText; // Getting text from the input field


    public List<Sprite> partImages = new List<Sprite>();

    public Dictionary<string, Sprite> imageDictionary = new Dictionary<string, Sprite>();

    public GridLayoutGroup Grid;
    public GameObject spawnParent;


    // These two enums allow us to choose between old and new methods
    public ImageLoadMethod imageLoadMethod;
    public AssetLoadMethod assetLoadMethod;

    public Items itemList;

    void Awake()
    {
        slotObjects = GameObject.FindGameObjectsWithTag("Slots");
        if (assetLoadMethod == AssetLoadMethod.FilePath)
        {
            // Load itemDescriptions.JSON at the beginning for ease
            string path = "Assets/Resources/Edited Human Parts/itemDescriptions.json";
            if (System.IO.File.Exists(path))
            {
                string existingJson = System.IO.File.ReadAllText(path);
                Debug.Log(existingJson);
                itemList = JsonUtility.FromJson<Items>(existingJson);

            }
        }

    }

    // Start is initial menu population
    void Start()
    {
        for (int i = 0; i < parts.Count; i++)
        {
            imageDictionary.Add(parts[i].name, partImages[i]);
        }

        // Initial list build
        RefreshMenu();

        // Listen if the search field changes
        inputField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { ValueChangeCheck(); });

    }

    void searchMenuItems(string searchValue)
    {

        // Then we need to build the new filtered list
        // Build filtered list
        List<GameObject> filteredParts = new List<GameObject>();
        for (int i = 0; i < menuItemList.Count; i++)

        {
            if (!menuItemList[i].name.ToLower().Contains(searchValue.ToLower()))
            {
                menuItemList[i].SetActive(false);
            }
            else
            {
                menuItemList[i].SetActive(true);
            }
        }

    }

    // Executes when the search event listener is called
    void ValueChangeCheck()
    {
        searchText = inputField.GetComponent<TMP_InputField>().text;

        // Debug.Log(searchText);

        searchMenuItems(searchText);
    }

    // This function instantiates the menu
    void RefreshMenu()
    {
        if (assetLoadMethod == AssetLoadMethod.List)
        {
            foreach (GameObject part in parts)
            {

                GameObject currentMenuItem = Instantiate(menuItem);
                currentMenuItem.transform.SetParent(Grid.transform, false);
                currentMenuItem.transform.localScale = new Vector3(0.24f, 0.24f, 0.24f);
                currentMenuItem.transform.localPosition = Vector3.zero;

                currentMenuItem.GetComponentInChildren<TextMeshProUGUI>().text = part.name;
                currentMenuItem.name = part.name;
                BoxCollider itemCollider = currentMenuItem.AddComponent<BoxCollider>(); // Add Box Collider
                itemCollider.size = new Vector3(100f, 100f, 1f);
                SpawnMenuItem smi = currentMenuItem.AddComponent<SpawnMenuItem>(); // Script responsible for spawning the menu item
                smi.menuItemName = currentMenuItem.name; // Set the menu item name 
                smi.menuItem = part; // Set the menu item to be spawned
                menuItemList.Add(currentMenuItem);
                smi.spawnParent = spawnParent;
            }
        }
        else if (assetLoadMethod == AssetLoadMethod.FilePath)
        {
            // Instantiate menu item for each slot object
            foreach (GameObject slot in slotObjects)
            {
                // Search for the body part with the same slot name 
                Item item = itemList.bodyParts.FirstOrDefault(x => x.name == slot.name);
                string itemName;
                if (item == null)
                {
                    itemName = "No item";
                    Debug.LogError("No Item: An entry was not found for " + slot.name);
                }
                else
                {
                    itemName = item.name; // Cache the name to avoid closure issues

                }


                GameObject currentMenuItem = Instantiate(menuItem);
                currentMenuItem.transform.SetParent(Grid.transform, false);
                currentMenuItem.transform.localScale = new Vector3(0.24f, 0.24f, 0.24f);
                currentMenuItem.transform.localPosition = Vector3.zero;

                currentMenuItem.GetComponentInChildren<TextMeshProUGUI>().text = itemName;
                currentMenuItem.name = itemName;
                BoxCollider itemCollider = currentMenuItem.AddComponent<BoxCollider>(); // Add Box Collider
                itemCollider.size = new Vector3(100f, 100f, 1f);
                SpawnMenuItem smi = currentMenuItem.AddComponent<SpawnMenuItem>(); // Script responsible for spawning the menu item
                smi.menuItemName = currentMenuItem.name; // Set the menu item name 
                // Load the addressable with the same item name
                Addressables.LoadAssetAsync<GameObject>(itemName).Completed += handle =>
                {
                    if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    {
                        smi.menuItem = (handle.Result);
                    }
                    else
                    {
                        Debug.Log("Failed to load" + itemName + " via Addressables.");
                    }
                };
                menuItemList.Add(currentMenuItem);
                smi.spawnParent = spawnParent;
            }
        }

        // Set each item's image


        for (int i = 0; i < menuItemList.Count; i++)
        {
            Image img = menuItemList[i].GetComponentInChildren<Image>();
            if (img == null)
            {
                Debug.LogWarning($"No Image component found in {menuItemList[i].name}");
                continue;
            }

            if (!imageDictionary.ContainsKey(menuItemList[i].name))
            {
                Debug.LogWarning($"imageDictionary missing key: {menuItemList[i].name}");
                continue;
            }

            // Old method uses dictionary

            if (imageLoadMethod == ImageLoadMethod.Dictionary)
            {

                Debug.Log("Loading " + menuItemList[i] + " from the image dictionary");
                img.sprite = imageDictionary[menuItemList[i].name];
            }

            // New method uses a filepath to search for the image in a folder
            else if (imageLoadMethod == ImageLoadMethod.FilePath)
            {
                string filepath = "C:/Users/super/Skeleton Puzzle/Assets/Resources/ObjectIcons/" + menuItemList[i].name.ToLower() + ".JPG"; // CASE SENSITIVE

                // New way of doing it is loading it from a filepath using the item name
                if (File.Exists(filepath))
                {

                    Debug.Log("Loading " + filepath);
                    byte[] fileData = File.ReadAllBytes(filepath);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.

                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                    img.sprite = sprite;

                }
                else
                {
                    Debug.Log("The image at " + filepath + " does not exist!");
                }
            }


        }


    }




}

